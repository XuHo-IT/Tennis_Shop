using BussinessObject;
using Repositories;

namespace Services
{
    public class OrderService : IOrderService
    {
        private static readonly HashSet<string> AllowedStatuses = new(new[]
        {
            "pending", "processing", "shipped", "completed", "cancelled"
        }, StringComparer.OrdinalIgnoreCase);

        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            if (userId <= 0)
                return Array.Empty<Order>();

            return await _orderRepository.GetOrdersByUserAsync(userId);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            ValidateOrderForCreation(order);
            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task<Order?> UpdateOrderStatusAsync(int orderId, string status)
        {
            if (orderId <= 0)
                return null;

            if (string.IsNullOrWhiteSpace(status) || !AllowedStatuses.Contains(status))
                throw new ArgumentException("Invalid order status");

            return await _orderRepository.UpdateOrderStatusAsync(orderId, status);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            if (id <= 0)
                return false;

            return await _orderRepository.DeleteOrderAsync(id);
        }

        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            if (userId <= 0 || productId <= 0)
                return false;

            var orders = await _orderRepository.GetOrdersByUserAsync(userId);
            
            // Check if user has any completed orders containing this product
            return orders.Any(order => 
                (order.Status?.ToLower() == "completed" || order.Status?.ToLower() == "shipped") &&
                order.OrderItems.Any(item => item.ProductId == productId));
        }

        private void ValidateOrderForCreation(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.UserId <= 0)
                throw new ArgumentException("UserId is required");

            if (order.OrderItems == null || !order.OrderItems.Any())
                throw new ArgumentException("Order must contain at least one item");

            foreach (var item in order.OrderItems)
            {
                if (item.ProductId <= 0)
                    throw new ArgumentException("Order item is missing ProductId");
                if (item.Quantity <= 0)
                    throw new ArgumentException("Order item quantity must be > 0");
                if (item.Price < 0)
                    throw new ArgumentException("Order item price must be >= 0");
            }
        }
    }
}


