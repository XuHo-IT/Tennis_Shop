using DataAccessLayer.Models;

namespace Repositories
{
    public interface ICartRepository
    {
        Task<Carts?> GetCartByUserIdAsync(int userId);
        Task<Carts?> GetCartByIdAsync(int cartId);
        Task<Carts> CreateCartAsync(int userId);
        Task<CartItems> AddItemToCartAsync(int cartId, int productId, int quantity, int? variantId = null);
        Task<CartItems?> UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task<bool> RemoveItemFromCartAsync(int cartItemId);
        Task<bool> ClearCartAsync(int cartId);
        Task<int> GetCartItemCountAsync(int cartId);
        Task<decimal> GetCartTotalAsync(int cartId);
    }
}
