using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;
using BussinessObject;

namespace TennisShop.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;
        private readonly IPayOSService _payOSService;

        public PaymentController(
            IOrderService orderService,
            ICartService cartService,
            IUserService userService,
            IPaymentService paymentService,
            IPayOSService payOSService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userService = userService;
            _paymentService = paymentService;
            _payOSService = payOSService;
        }

        // GET: /Payment/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.User = user;
            ViewBag.Cart = cart;
            
            // Calculate totals
            var subtotal = cart.CartItems.Sum(item => item.Quantity * item.UnitPrice);
            var tax = subtotal * 0.1m; // 10% tax
            var total = subtotal + tax;

            ViewBag.Subtotal = subtotal;
            ViewBag.Tax = tax;
            ViewBag.Total = total;

            return View();
        }

        // POST: /Payment/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string shippingAddress, string phone, string paymentMethod)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(shippingAddress) || string.IsNullOrWhiteSpace(phone))
            {
                TempData["ErrorMessage"] = "Shipping address and phone are required.";
                return RedirectToAction(nameof(Checkout));
            }

            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "pending",
                ShippingAddress = shippingAddress,
                Phone = phone,
                OrderItems = cart.CartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    VariantId = item.VariantId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                }).ToList()
            };

            // Calculate total
            var subtotal = cart.CartItems.Sum(item => item.Quantity * item.UnitPrice);
            var tax = subtotal * 0.1m;
            order.TotalAmount = subtotal + tax;

            // Create order
            var createdOrder = await _orderService.CreateOrderAsync(order);

            // Reload order with full details including Products
            var orderWithDetails = await _orderService.GetOrderByIdAsync(createdOrder.Id);
            if (orderWithDetails == null)
            {
                TempData["ErrorMessage"] = "Error creating order. Please try again.";
                return RedirectToAction("Index", "Cart");
            }

            // Create payment record
            var payment = new Payment
            {
                OrderId = createdOrder.Id,
                PaymentMethod = paymentMethod,
                PaymentStatus = paymentMethod == "COD" ? "pending" : "pending",
                Amount = order.TotalAmount,
                PaidAt = null
            };

            await _paymentService.CreatePaymentAsync(payment);

            // Clear cart
            await _cartService.ClearCartAsync(userId);

            // Handle payment method
            if (paymentMethod == "PayOS")
            {
                // Debug: Log order details before calling PayOS
                Console.WriteLine($"[PaymentController] About to create PayOS link for Order #{orderWithDetails.Id}");
                Console.WriteLine($"[PaymentController] OrderItems count: {orderWithDetails.OrderItems?.Count ?? 0}");
                Console.WriteLine($"[PaymentController] TotalAmount: {orderWithDetails.TotalAmount}");
                
                // Tạo link thanh toán PayOS
                var paymentUrl = await _payOSService.CreatePaymentLinkAsync(orderWithDetails);
                
                Console.WriteLine($"[PaymentController] PayOS returned URL: {paymentUrl ?? "NULL"}");
                
                if (!string.IsNullOrEmpty(paymentUrl))
                {
                    return Redirect(paymentUrl);
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to process PayOS payment. Please check server logs for details.";
                    return RedirectToAction("Details", "Order", new { id = createdOrder.Id });
                }
            }
            else // COD
            {
                TempData["SuccessMessage"] = "Order placed successfully! You will pay when you receive the order.";
                return RedirectToAction("Details", "Order", new { id = createdOrder.Id });
            }
        }

        // GET: /Payment/PaymentSuccess
        public async Task<IActionResult> PaymentSuccess(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || order.UserId != userId)
            {
                return Forbid();
            }

            // Update order status
            await _orderService.UpdateOrderStatusAsync(orderId, "processing");

            // Update payment status
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (payment != null)
            {
                await _paymentService.UpdatePaymentStatusAsync(payment.Id, "completed");
            }

            return View(order);
        }

        // GET: /Payment/PaymentCancel
        public async Task<IActionResult> PaymentCancel(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || order.UserId != userId)
            {
                return Forbid();
            }

            return View(order);
        }

        // GET: /Payment/RetryPayment/6
        public async Task<IActionResult> RetryPayment(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId) || order.UserId != userId)
            {
                return Forbid();
            }

            // Only allow retry for pending orders
            if (!string.Equals(order.Status, "pending", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Can only retry payment for pending orders.";
                return RedirectToAction("Details", "Order", new { id });
            }

            // Generate PayOS payment link
            var paymentUrl = await _payOSService.CreatePaymentLinkAsync(order);
            
            if (!string.IsNullOrEmpty(paymentUrl))
            {
                return Redirect(paymentUrl);
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to process PayOS payment. Please try again later.";
                return RedirectToAction("Details", "Order", new { id });
            }
        }
    }
}

