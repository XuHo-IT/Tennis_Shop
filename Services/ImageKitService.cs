using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Services
{
    public class ImageKitService : IImageKitService
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlEndpoint;
        private readonly string _publicKey;
        private readonly string _privateKey;

        public ImageKitService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _urlEndpoint = configuration["ImageKit:UrlEndpoint"] ?? "https://ik.imagekit.io/6tazphtet3";
            _publicKey = configuration["ImageKit:PublicKey"] ?? "public_6azZ/lFmUjQ96LVPfmJRJlc1iHc=";
            _privateKey = configuration["ImageKit:PrivateKey"] ?? "private_xbut0n/7aAMsRjJTOHYlxpz47RQ=";
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string folder = "products")
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Add the file
                var fileContent = new StreamContent(imageStream);
                content.Add(fileContent, "file", fileName);

                // Optional fields
                content.Add(new StringContent(fileName), "fileName");
                if (!string.IsNullOrEmpty(folder))
                {
                    content.Add(new StringContent(folder), "folder");
                }

                // Use Basic Authentication (Private API key)
                var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_privateKey}:"));
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {authHeaderValue}");

                var response = await _httpClient.PostAsync("https://upload.imagekit.io/api/v1/files/upload", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"ImageKit upload failed with status {response.StatusCode}: {responseContent}");

                var uploadResult = JsonSerializer.Deserialize<ImageKitUploadResponse>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (uploadResult == null)
                    throw new Exception("ImageKit upload succeeded but returned null response.");

                // ✅ If ImageKit returned a custom domain (like cdn.sport.com),
                // rebuild the URL to always use the official ik.imagekit.io endpoint
                if (!string.IsNullOrEmpty(uploadResult.FilePath))
                {
                    string officialCdnUrl = $"{_urlEndpoint.TrimEnd('/')}{uploadResult.FilePath}";
                    return officialCdnUrl;
                }

                // fallback
                return uploadResult.Url ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading image to ImageKit: {ex.Message}", ex);
            }
        }


        public async Task<bool> DeleteImageAsync(string imageId)
        {
            try
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var token = GenerateToken(imageId, timestamp);

                var deleteData = new
                {
                    fileId = imageId,
                    publicKey = _publicKey,
                    timestamp = timestamp,
                    token = token
                };

                var json = JsonSerializer.Serialize(deleteData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://upload.imagekit.io/api/v1/files/delete", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting image from ImageKit: {ex.Message}", ex);
            }
        }

        public Task<string> GetImageUrlAsync(string imagePathOrUrl, int? width = null, int? height = null)
        {
            if (string.IsNullOrWhiteSpace(imagePathOrUrl))
                return Task.FromResult(string.Empty);

            string finalUrl;

            // ✅ If it's already a full URL
            if (imagePathOrUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                imagePathOrUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                // Normalize any custom CDN URL to the official ImageKit endpoint
                finalUrl = Regex.Replace(
                    imagePathOrUrl,
                    @"https:\/\/[^\/]+\/",
                    $"{_urlEndpoint.TrimEnd('/')}/"
                );
            }
            else
            {
                // Otherwise, treat as relative path or image ID
                finalUrl = $"{_urlEndpoint.TrimEnd('/')}/{imagePathOrUrl.TrimStart('/')}";
            }

            // ✅ Apply optional transformations (resize)
            if (width.HasValue || height.HasValue)
            {
                var transformations = new List<string>();
                if (width.HasValue) transformations.Add($"w-{width}");
                if (height.HasValue) transformations.Add($"h-{height}");

                var separator = finalUrl.Contains('?') ? '&' : '?';

                if (finalUrl.Contains("tr="))
                {
                    finalUrl = Regex.Replace(
                        finalUrl,
                        @"tr=([^&]*)",
                        match => $"tr={match.Groups[1].Value},{string.Join(",", transformations)}"
                    );
                }
                else
                {
                    finalUrl += $"{separator}tr={string.Join(",", transformations)}";
                }
            }

            return Task.FromResult(finalUrl);
        }

        public Task<string> GetTransformedImageUrlAsync(string imageId, string transformation)
        {
            var url = $"{_urlEndpoint.TrimEnd('/')}/{imageId}?tr={transformation}";
            return Task.FromResult(url);
        }

        private string GenerateToken(string fileName, long timestamp)
        {
            // ImageKit token format: HMAC-SHA1 of (timestamp + publicKey + fileName)
            var message = $"{timestamp}{_publicKey}{fileName}";
            using var hmac = new System.Security.Cryptography.HMACSHA1(Encoding.UTF8.GetBytes(_privateKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public class ImageKitUploadResponse
    {
        public string FileId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int Size { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
