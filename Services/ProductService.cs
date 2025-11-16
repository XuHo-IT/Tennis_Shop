using BussinessObject;
using Repositories;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }
        public async Task<IEnumerable<ProductCategory>> GetAllCategorysAsync()
        {
            var categories = await _productRepository.GetAllCategories();
            return categories ?? new List<ProductCategory>();
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            var brands = await _productRepository.GetAllBrands();
            return brands ?? new List<Brand>();
        }


        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name is required");

            if (product.BasePrice <= 0)
                throw new ArgumentException("Product price must be greater than 0");

            if (product.Stock < 0)
                throw new ArgumentException("Stock cannot be negative");

            return await _productRepository.CreateProductAsync(product);
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            if (product.Id <= 0)
                return null;

            // Business logic validation
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name is required");

            if (product.BasePrice <= 0)
                throw new ArgumentException("Product price must be greater than 0");

            if (product.Stock < 0)
                throw new ArgumentException("Stock cannot be negative");

            return await _productRepository.UpdateProductAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0)
                return false;

            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
                return new List<Product>();

            return await _productRepository.GetProductsByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllProductsAsync();

            return await _productRepository.SearchProductsAsync(searchTerm.Trim());
        }

    }
}
