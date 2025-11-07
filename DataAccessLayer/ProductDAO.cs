using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;

namespace DataAccessLayer
{
    public class ProductDAO
    {
        private readonly SportManagementContext _context;

        public ProductDAO(SportManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductAsync()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .Where(p => p.IsActive == true)
                .ToListAsync();
        }
        public async Task<IEnumerable<ProductCategory>> GetAllCategoryAsync()
        {
            return await _context.ProductCategories            
                .ToListAsync();
        }
        public async Task<IEnumerable<Brand>> GetAllBrandAsync()
        {
            return await _context.Brands
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.Now;
            product.IsActive = true;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (existingProduct == null)
                return null;

            // Update scalar properties
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.BrandId = product.BrandId;
            existingProduct.BasePrice = product.BasePrice;
            existingProduct.DiscountPercent = product.DiscountPercent;
            existingProduct.Stock = product.Stock;
            existingProduct.IsActive = product.IsActive;

            // --- Handle ProductImages updates ---
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                // Remove existing primary/main images only if a new one is provided
                var existingPrimaryImages = existingProduct.ProductImages
                     .Where(img => (img.IsPrimary ?? false) || (img.IsMain ?? false))
                     .ToList();


                foreach (var existingImg in existingPrimaryImages)
                {
                    _context.ProductImages.Remove(existingImg);
                }

                foreach (var newImage in product.ProductImages)
                {
                    // If this image already exists in DB (has Id > 0), reattach instead of inserting
                    if (newImage.Id > 0)
                    {
                        _context.Entry(newImage).State = EntityState.Unchanged;
                        existingProduct.ProductImages.Add(newImage);
                    }
                    else
                    {
                        // New image → mark for insertion
                        newImage.ProductId = existingProduct.Id;
                        _context.ProductImages.Add(newImage);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return existingProduct;
        }


        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryId == categoryId && p.IsActive == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive == true && 
                           (p.Name.Contains(searchTerm) || 
                            p.Description.Contains(searchTerm)))
                .ToListAsync();
        }
    }
}
