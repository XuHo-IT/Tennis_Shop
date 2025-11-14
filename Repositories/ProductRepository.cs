
using DataAccessLayer;
using BussinessObject;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDAO _productDAO;

        public ProductRepository(ProductDAO productDAO)
        {
            _productDAO = productDAO;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productDAO.GetAllProductAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productDAO.GetProductByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            return await _productDAO.CreateProductAsync(product);
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            return await _productDAO.UpdateProductAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productDAO.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _productDAO.GetProductsByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _productDAO.SearchProductsAsync(searchTerm);
        }
        public async Task<IEnumerable<ProductCategory>> GetAllCategories()
        {
            return await _productDAO.GetAllCategoryAsync();
        }
        public async Task<IEnumerable<Brand>> GetAllBrands()
        {
            return await _productDAO.GetAllBrandAsync();
        }
    }
}
