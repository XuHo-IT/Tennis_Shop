using BussinessObject;
using DataAccessLayer.Models;

namespace Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAO _orderDAO;

        public OrderRepository(OrderDAO orderDAO)
        {
            _orderDAO = orderDAO;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderDAO.GetAllOrdersAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderDAO.GetOrderByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _orderDAO.GetOrdersByUserAsync(userId);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            return await _orderDAO.CreateOrderAsync(order);
        }

        public async Task<Order?> UpdateOrderStatusAsync(int orderId, string status)
        {
            return await _orderDAO.UpdateOrderStatusAsync(orderId, status);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            return await _orderDAO.DeleteOrderAsync(id);
        }
    }
}


