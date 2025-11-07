using DataAccessLayer.Models;

namespace Services
{
    public interface ICartService
    {
        Task<Carts?> GetCartByUserIdAsync(int userId);
        Task<Carts?> GetCartByIdAsync(int cartId);
        Task<Carts> GetOrCreateCartAsync(int userId);
        Task<CartItems> AddItemToCartAsync(int userId, int productId, int quantity, int? variantId = null);
        Task<CartItems?> UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task<bool> RemoveItemFromCartAsync(int cartItemId);
        Task<bool> ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
    }
}
