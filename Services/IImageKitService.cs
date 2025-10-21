using System.Threading.Tasks;

namespace Services
{
    public interface IImageKitService
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName, string folder = "products");
        Task<bool> DeleteImageAsync(string imageId);
        Task<string> GetImageUrlAsync(string imageId, int? width = null, int? height = null);
        Task<string> GetTransformedImageUrlAsync(string imageId, string transformation);
    }
}
