# Tích hợp PayOS vào TennisShop

## Tổng quan

Dự án TennisShop đã được tích hợp PayOS payment gateway sử dụng SDK chính thức `payOS` version 1.0.9.

## Cấu trúc tích hợp

### 1. Package đã cài đặt
- **payOS** v1.0.9 trong `Services/Services.csproj`

### 2. Các file mới được tạo

#### Services Layer
- `Services/IPayOSService.cs` - Interface cho PayOS service
- `Services/PayOSService.cs` - Implementation của PayOS service sử dụng Net.payOS SDK

#### Views
- `TennisShop/Views/Payment/PaymentSuccess.cshtml` - Trang hiển thị khi thanh toán thành công
- `TennisShop/Views/Payment/PaymentCancel.cshtml` - Trang hiển thị khi thanh toán bị hủy

### 3. Các file đã được cập nhật

- `TennisShop/Program.cs` - Đăng ký `IPayOSService` vào DI container
- `TennisShop/Controllers/PaymentController.cs` - Tích hợp PayOSService và xử lý callback
- `TennisShop/appsettings.json` - Cấu hình PayOS credentials

## Cấu hình

### appsettings.json

```json
{
  "PayOS": {
    "ClientId": "a7edd952-9f4c-4a04-9fc9-8c7f39e6a232",
    "ApiKey": "e1b31221-a919-41d3-8d73-0454a7ee0c26",
    "ChecksumKey": "799e0807ceb33449c755e1209b459f64c00d8f4f9929ebc901462262b2e91afb",
    "SuccessUrl": "https://localhost:7152/Payment/PaymentSuccess?orderId=",
    "CancelUrl": "https://localhost:7152/Payment/PaymentCancel?orderId="
  }
}
```

**Lưu ý:** 
- URLs kết thúc bằng `?orderId=` để PayOSService có thể nối thêm orderId
- Khi deploy production, cần thay đổi URLs thành domain thật

### Production Configuration

Khi deploy lên production (ví dụ: https://veggies-qiw1.onrender.com), cập nhật URLs:

```json
{
  "PayOS": {
    "SuccessUrl": "https://veggies-qiw1.onrender.com/Payment/PaymentSuccess?orderId=",
    "CancelUrl": "https://veggies-qiw1.onrender.com/Payment/PaymentCancel?orderId="
  }
}
```

## Luồng thanh toán

### 1. Checkout Flow

```
User → Checkout Page → Select PayOS → Place Order → PayOSService.CreatePaymentLinkAsync()
→ Redirect to PayOS Gateway → User completes payment → Callback to Success/Cancel URL
```

### 2. Success Flow

```
PayOS → /Payment/PaymentSuccess?orderId={id}
→ Update order status to "processing"
→ Update payment status to "completed"
→ Display PaymentSuccess view
```

### 3. Cancel Flow

```
PayOS → /Payment/PaymentCancel?orderId={id}
→ Keep order status as "pending"
→ Display PaymentCancel view with "Retry Payment" button
```

## API PayOSService

### CreatePaymentLinkAsync(Order order)

Tạo link thanh toán PayOS cho một đơn hàng.

**Parameters:**
- `order`: Đối tượng Order với đầy đủ OrderItems và Product details

**Returns:**
- `string?`: URL checkout của PayOS hoặc null nếu có lỗi

**Example:**
```csharp
var paymentUrl = await _payOSService.CreatePaymentLinkAsync(order);
if (!string.IsNullOrEmpty(paymentUrl))
{
    return Redirect(paymentUrl);
}
```

## Các tính năng chính

### 1. Tự động tạo link thanh toán
- Sử dụng `order.Id` làm order code duy nhất
- Tự động tính tổng tiền từ OrderItems
- Tự động tạo danh sách items với tên sản phẩm, số lượng và giá

### 2. Callback handling
- Tự động append orderId vào success/cancel URLs
- Validate user ownership của order
- Update order và payment status tự động

### 3. Retry Payment
- Người dùng có thể thử lại thanh toán từ trang Cancel
- Hoặc từ Order Details nếu order status là "pending"

### 4. Error Handling
- Try-catch trong CreatePaymentLinkAsync để xử lý lỗi
- Log chi tiết lỗi vào Console
- Return null khi có lỗi để controller xử lý

## Testing

### Local Testing

1. Chạy ứng dụng:
```bash
dotnet run --project TennisShop/TennisShop.csproj
```

2. Truy cập: https://localhost:7152

3. Flow test:
   - Đăng nhập
   - Thêm sản phẩm vào giỏ hàng
   - Checkout và chọn PayOS
   - Điền thông tin giao hàng
   - Place Order
   - Được redirect đến PayOS Gateway
   - Test thanh toán/hủy

### PayOS Sandbox

PayOS cung cấp sandbox environment để test. Chi tiết xem tại: https://payos.vn/docs

## Troubleshooting

### Lỗi "Unable to process PayOS payment"

**Nguyên nhân có thể:**
1. PayOS credentials không đúng
2. Order không có OrderItems hoặc Product details
3. TotalAmount bằng 0 hoặc âm
4. Không kết nối được PayOS API

**Kiểm tra:**
- Xem Console logs để biết chi tiết lỗi
- Verify PayOS credentials trong appsettings.json
- Đảm bảo Order được load đầy đủ với OrderItems và Products

### Callback URL không hoạt động

**Nguyên nhân:**
- URL trong appsettings.json không đúng
- PayOS không thể truy cập localhost (khi test local)

**Giải pháp:**
- Sử dụng ngrok hoặc tunneling service để expose localhost
- Hoặc deploy lên server có public URL

## Best Practices

1. **Luôn load đầy đủ Order details** trước khi gọi CreatePaymentLinkAsync
2. **Validate order ownership** trước khi xử lý callback
3. **Log errors** để dễ debug
4. **Update URLs** khi chuyển environment (dev → staging → production)
5. **Kiểm tra payment status** trước khi fulfill order

## Support

Tài liệu PayOS: https://payos.vn/docs
SDK Documentation: https://github.com/payOSHQ/payos-lib-dotnet

