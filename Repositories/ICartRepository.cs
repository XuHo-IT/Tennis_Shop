using BussinessObject;

namespace Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Cart?> GetCartByIdAsync(int cartId);
        Task<Cart> CreateCartAsync(int userId);
        Task<CartItem> AddItemToCartAsync(int cartId, int productId, int quantity, int? variantId = null);
        Task<CartItem?> UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task<bool> RemoveItemFromCartAsync(int cartItemId);
        Task<bool> ClearCartAsync(int cartId);
        Task<int> GetCartItemCountAsync(int cartId);
        Task<decimal> GetCartTotalAsync(int cartId);
    }
}
