using BussinessObject;

namespace Services
{
    public interface IPayOSService
    {
        /// <summary>
        /// Tạo link thanh toán PayOS cho một đơn hàng.
        /// </summary>
        /// <param name="order">Đơn hàng cần thanh toán</param>
        /// <returns>Một chuỗi URL thanh toán hoặc null nếu có lỗi.</returns>
        Task<string?> CreatePaymentLinkAsync(Order order);
    }
}

