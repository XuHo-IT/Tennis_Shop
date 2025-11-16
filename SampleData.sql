-- =============================================
-- Sample Data for Sport Management System
-- Tennis Shop Database
-- =============================================

USE Sport;
GO

-- Clear existing data (optional - uncomment if needed)
/*
DELETE FROM order_items;
DELETE FROM payments;
DELETE FROM reviews;
DELETE FROM orders;
DELETE FROM product_images;
DELETE FROM product_variants;
DELETE FROM products;
DELETE FROM newsletter_subscribers;
DELETE FROM promo_codes;
DELETE FROM users;
DELETE FROM product_categories;
DELETE FROM brands;
DELETE FROM user_roles;
*/

-- =============================================
-- 1. USER ROLES
-- =============================================
SET IDENTITY_INSERT user_roles ON;

INSERT INTO user_roles (id, name) VALUES
(1, 'admin'),
(2, 'user');

SET IDENTITY_INSERT user_roles OFF;
GO

-- =============================================
-- 2. BRANDS
-- =============================================
SET IDENTITY_INSERT brands ON;

INSERT INTO brands (id, name, description) VALUES
(1, 'Wilson', 'Wilson Tennis - Leading tennis equipment manufacturer'),
(2, 'Babolat', 'Babolat - French tennis equipment and apparel brand'),
(3, 'Head', 'Head - Premium tennis rackets and equipment'),
(4, 'Yonex', 'Yonex - Japanese sports equipment manufacturer'),
(5, 'Prince', 'Prince - American tennis equipment brand'),
(6, 'Nike', 'Nike - Athletic apparel and footwear'),
(7, 'Adidas', 'Adidas - Sports apparel and accessories');

SET IDENTITY_INSERT brands OFF;
GO

-- =============================================
-- 3. PRODUCT CATEGORIES
-- =============================================
SET IDENTITY_INSERT product_categories ON;

INSERT INTO product_categories (id, name, description, parent_id) VALUES
-- Main Categories
(1, 'Tennis Rackets', 'Professional and recreational tennis rackets', NULL),
(2, 'Tennis Shoes', 'Tennis-specific footwear', NULL),
(3, 'Tennis Apparel', 'Clothing for tennis players', NULL),
(4, 'Tennis Accessories', 'Tennis balls, bags, and other accessories', NULL),
-- Sub-categories
(5, 'Professional Rackets', 'High-performance rackets for advanced players', 1),
(6, 'Beginner Rackets', 'Rackets suitable for beginners', 1),
(7, 'Men Shoes', 'Men''s tennis shoes', 2),
(8, 'Women Shoes', 'Women''s tennis shoes', 2),
(9, 'Men Clothing', 'Men''s tennis apparel', 3),
(10, 'Women Clothing', 'Women''s tennis apparel', 3),
(11, 'Tennis Balls', 'Professional and practice tennis balls', 4),
(12, 'Bags & Backpacks', 'Tennis bags and backpacks', 4);

SET IDENTITY_INSERT product_categories OFF;
GO

-- =============================================
-- 4. USERS
-- =============================================
SET IDENTITY_INSERT users ON;

-- Note: Passwords are stored as plain text (no hashing)
-- Admin Password: admin123
-- User Passwords: john123, jane123, mike123, sarah123
INSERT INTO users (id, full_name, email, password_hash, phone, address, role_id, created_at) VALUES
(1, 'Admin User', 'admin@tennisshop.com', 'admin123', '0123456789', '123 Admin Street, City', 1, '2024-01-01 10:00:00'),
(2, 'John Doe', 'john.doe@email.com', 'john123', '0987654321', '456 Main Street, District 1', 2, '2024-01-15 14:30:00'),
(3, 'Jane Smith', 'jane.smith@email.com', 'jane123', '0912345678', '789 Park Avenue, District 2', 2, '2024-02-01 09:15:00'),
(4, 'Mike Johnson', 'mike.j@email.com', 'mike123', '0923456789', '321 Sports Lane, District 3', 2, '2024-02-10 16:45:00'),
(5, 'Sarah Williams', 'sarah.w@email.com', 'sarah123', '0934567890', '654 Tennis Court Road, District 7', 2, '2024-02-20 11:20:00');

SET IDENTITY_INSERT users OFF;
GO

-- =============================================
-- 5. PRODUCTS
-- =============================================
SET IDENTITY_INSERT products ON;

INSERT INTO products (id, name, description, category_id, brand_id, base_price, discount_percent, stock, is_active, created_at) VALUES
-- Tennis Rackets
(1, 'Wilson Pro Staff RF97', 'Roger Federer signature racket with exceptional control and feel', 5, 1, 250.00, 0, 15, 1, '2024-01-10 08:00:00'),
(2, 'Babolat Pure Drive 2024', 'Powerful racket perfect for aggressive baseline players', 5, 2, 220.00, 10, 20, 1, '2024-01-12 09:30:00'),
(3, 'Head Speed Pro', 'High-performance racket for advanced players seeking speed', 5, 3, 240.00, 0, 18, 1, '2024-01-15 10:15:00'),
(4, 'Wilson Blade 98', 'Perfect balance of control and power', 6, 1, 180.00, 15, 25, 1, '2024-01-18 11:00:00'),
(5, 'Yonex EZONE 100', 'Comfortable racket with excellent vibration dampening', 6, 4, 200.00, 0, 22, 1, '2024-01-20 14:20:00'),

-- Tennis Shoes
(6, 'Nike Court Vapor', 'Lightweight tennis shoes with responsive cushioning', 7, 6, 120.00, 0, 30, 1, '2024-01-22 15:30:00'),
(7, 'Adidas Barricade', 'Durable tennis shoes with superior stability', 7, 7, 130.00, 10, 28, 1, '2024-01-25 16:00:00'),
(8, 'Nike Women Air Zoom', 'Women''s tennis shoes with Air Zoom technology', 8, 6, 115.00, 0, 35, 1, '2024-01-28 09:45:00'),
(9, 'Adidas Stella Court', 'Women''s tennis shoes with boost cushioning', 8, 7, 125.00, 5, 32, 1, '2024-02-01 10:30:00'),

-- Tennis Apparel
(10, 'Nike Men Tennis Polo', 'Breathable men''s tennis polo shirt', 9, 6, 45.00, 0, 40, 1, '2024-02-03 11:15:00'),
(11, 'Adidas Men Shorts', 'Performance men''s tennis shorts', 9, 7, 35.00, 10, 45, 1, '2024-02-05 12:00:00'),
(12, 'Nike Women Skirt', 'Stylish women''s tennis skirt with built-in shorts', 10, 6, 55.00, 0, 38, 1, '2024-02-07 13:20:00'),
(13, 'Adidas Women Tank Top', 'Comfortable women''s tennis tank top', 10, 7, 40.00, 15, 42, 1, '2024-02-10 14:10:00'),

-- Tennis Accessories
(14, 'Wilson Pro Staff Tennis Balls (4 pack)', 'Professional grade tennis balls', 11, 1, 12.00, 0, 100, 1, '2024-02-12 15:00:00'),
(15, 'Babolat Tennis Backpack', 'Large capacity tennis backpack', 12, 2, 60.00, 20, 25, 1, '2024-02-15 16:30:00'),
(16, 'Head Tennis Bag', 'Professional tennis bag with multiple compartments', 12, 3, 75.00, 10, 20, 1, '2024-02-18 10:00:00');

SET IDENTITY_INSERT products OFF;
GO

-- =============================================
-- 6. PRODUCT VARIANTS
-- =============================================
SET IDENTITY_INSERT product_variants ON;

INSERT INTO product_variants (id, product_id, color, size, sku, stock, price) VALUES
-- Rackets with grip sizes
(1, 1, 'Black/Red', '4 3/8', 'WIL-PS-RF97-438', 5, 250.00),
(2, 1, 'Black/Red', '4 1/2', 'WIL-PS-RF97-450', 5, 250.00),
(3, 1, 'Black/Red', '4 5/8', 'WIL-PS-RF97-462', 5, 250.00),
(4, 2, 'Blue/White', '4 3/8', 'BAB-PD-2024-438', 7, 220.00),
(5, 2, 'Blue/White', '4 1/2', 'BAB-PD-2024-450', 7, 220.00),
(6, 2, 'Blue/White', '4 5/8', 'BAB-PD-2024-462', 6, 220.00),
(7, 3, 'Orange/Black', '4 3/8', 'HED-SP-PRO-438', 6, 240.00),
(8, 3, 'Orange/Black', '4 1/2', 'HED-SP-PRO-450', 6, 240.00),
(9, 3, 'Orange/Black', '4 5/8', 'HED-SP-PRO-462', 6, 240.00),

-- Shoes with sizes
(10, 6, 'White/Black', '8', 'NIKE-CV-M-8', 5, 120.00),
(11, 6, 'White/Black', '9', 'NIKE-CV-M-9', 5, 120.00),
(12, 6, 'White/Black', '10', 'NIKE-CV-M-10', 5, 120.00),
(13, 6, 'White/Black', '11', 'NIKE-CV-M-11', 5, 120.00),
(14, 7, 'Blue/White', '8', 'ADI-BAR-M-8', 5, 130.00),
(15, 7, 'Blue/White', '9', 'ADI-BAR-M-9', 5, 130.00),
(16, 7, 'Blue/White', '10', 'ADI-BAR-M-10', 5, 130.00),
(17, 8, 'Pink/White', '6', 'NIKE-AZ-W-6', 6, 115.00),
(18, 8, 'Pink/White', '7', 'NIKE-AZ-W-7', 6, 115.00),
(19, 8, 'Pink/White', '8', 'NIKE-AZ-W-8', 6, 115.00),
(20, 9, 'Purple/Gray', '6', 'ADI-SC-W-6', 5, 125.00),
(21, 9, 'Purple/Gray', '7', 'ADI-SC-W-7', 5, 125.00),
(22, 9, 'Purple/Gray', '8', 'ADI-SC-W-8', 5, 125.00),

-- Apparel with sizes and colors
(23, 10, 'White', 'M', 'NIKE-POLO-M-W', 10, 45.00),
(24, 10, 'White', 'L', 'NIKE-POLO-L-W', 10, 45.00),
(25, 10, 'Blue', 'M', 'NIKE-POLO-M-B', 10, 45.00),
(26, 10, 'Blue', 'L', 'NIKE-POLO-L-B', 10, 45.00),
(27, 11, 'Black', 'M', 'ADI-SHT-M-BLK', 12, 35.00),
(28, 11, 'Black', 'L', 'ADI-SHT-L-BLK', 12, 35.00),
(29, 11, 'Navy', 'M', 'ADI-SHT-M-NVY', 11, 35.00),
(30, 12, 'White', 'S', 'NIKE-SKT-W-S', 8, 55.00),
(31, 12, 'White', 'M', 'NIKE-SKT-W-M', 8, 55.00),
(32, 12, 'Pink', 'S', 'NIKE-SKT-P-S', 8, 55.00),
(33, 12, 'Pink', 'M', 'NIKE-SKT-P-M', 8, 55.00),
(34, 13, 'Black', 'S', 'ADI-TNK-W-S', 10, 40.00),
(35, 13, 'Black', 'M', 'ADI-TNK-W-M', 10, 40.00),
(36, 13, 'Purple', 'S', 'ADI-TNK-W-P-S', 10, 40.00),
(37, 13, 'Purple', 'M', 'ADI-TNK-W-P-M', 12, 40.00);

SET IDENTITY_INSERT product_variants OFF;
GO

-- =============================================
-- 7. PRODUCT IMAGES
-- =============================================
SET IDENTITY_INSERT product_images ON;

INSERT INTO product_images (id, product_id, image_url, is_main) VALUES
-- Product 1 (Wilson Pro Staff RF97)
(1, 1, 'https://example.com/images/wilson-prostaff-1.jpg', 1),
(2, 1, 'https://example.com/images/wilson-prostaff-2.jpg', 0),
(3, 1, 'https://example.com/images/wilson-prostaff-3.jpg', 0),
-- Product 2 (Babolat Pure Drive 2024)
(4, 2, 'https://example.com/images/babolat-puredrive-1.jpg', 1),
(5, 2, 'https://example.com/images/babolat-puredrive-2.jpg', 0),
-- Product 3 (Head Speed Pro)
(6, 3, 'https://example.com/images/head-speed-1.jpg', 1),
(7, 3, 'https://example.com/images/head-speed-2.jpg', 0),
-- Product 4 (Wilson Blade 98)
(8, 4, 'https://example.com/images/wilson-blade-1.jpg', 1),
(9, 4, 'https://example.com/images/wilson-blade-2.jpg', 0),
-- Product 5 (Yonex EZONE 100)
(10, 5, 'https://example.com/images/yonex-ezone-1.jpg', 1),
(11, 5, 'https://example.com/images/yonex-ezone-2.jpg', 0),
-- Product 6 (Nike Court Vapor - Men Shoes)
(12, 6, 'https://example.com/images/nike-court-vapor-1.jpg', 1),
(13, 6, 'https://example.com/images/nike-court-vapor-2.jpg', 0),
(14, 6, 'https://example.com/images/nike-court-vapor-3.jpg', 0),
-- Product 7 (Adidas Barricade - Men Shoes)
(15, 7, 'https://example.com/images/adidas-barricade-1.jpg', 1),
(16, 7, 'https://example.com/images/adidas-barricade-2.jpg', 0),
-- Product 8 (Nike Women Air Zoom)
(17, 8, 'https://example.com/images/nike-women-airzoom-1.jpg', 1),
(18, 8, 'https://example.com/images/nike-women-airzoom-2.jpg', 0),
-- Product 9 (Adidas Stella Court - Women Shoes)
(19, 9, 'https://example.com/images/adidas-stella-1.jpg', 1),
(20, 9, 'https://example.com/images/adidas-stella-2.jpg', 0),
-- Product 10 (Nike Men Tennis Polo)
(21, 10, 'https://example.com/images/nike-polo-1.jpg', 1),
(22, 10, 'https://example.com/images/nike-polo-2.jpg', 0),
-- Product 11 (Adidas Men Shorts)
(23, 11, 'https://example.com/images/adidas-shorts-1.jpg', 1),
(24, 11, 'https://example.com/images/adidas-shorts-2.jpg', 0),
-- Product 12 (Nike Women Skirt)
(25, 12, 'https://example.com/images/nike-skirt-1.jpg', 1),
(26, 12, 'https://example.com/images/nike-skirt-2.jpg', 0),
-- Product 13 (Adidas Women Tank Top)
(27, 13, 'https://example.com/images/adidas-tank-1.jpg', 1),
(28, 13, 'https://example.com/images/adidas-tank-2.jpg', 0),
-- Product 14 (Wilson Pro Staff Tennis Balls)
(29, 14, 'https://example.com/images/tennis-balls-1.jpg', 1),
(30, 14, 'https://example.com/images/tennis-balls-2.jpg', 0),
-- Product 15 (Babolat Tennis Backpack)
(31, 15, 'https://example.com/images/babolat-backpack-1.jpg', 1),
(32, 15, 'https://example.com/images/babolat-backpack-2.jpg', 0),
(33, 15, 'https://example.com/images/babolat-backpack-3.jpg', 0),
-- Product 16 (Head Tennis Bag)
(34, 16, 'https://example.com/images/head-tennis-bag-1.jpg', 1),
(35, 16, 'https://example.com/images/head-tennis-bag-2.jpg', 0),
(36, 16, 'https://example.com/images/head-tennis-bag-3.jpg', 0);

SET IDENTITY_INSERT product_images OFF;
GO

-- =============================================
-- 8. ORDERS
-- =============================================
SET IDENTITY_INSERT orders ON;

INSERT INTO orders (id, user_id, order_date, status, shipping_address, phone, total_amount) VALUES
(1, 2, '2024-02-25 10:30:00', 'completed', '456 Main Street, District 1, Ho Chi Minh City', '0987654321', 250.00),
(2, 3, '2024-02-26 14:20:00', 'pending', '789 Park Avenue, District 2, Ho Chi Minh City', '0912345678', 355.00),
(3, 2, '2024-02-27 09:15:00', 'processing', '456 Main Street, District 1, Ho Chi Minh City', '0987654321', 240.00),
(4, 4, '2024-02-28 16:45:00', 'pending', '321 Sports Lane, District 3, Ho Chi Minh City', '0923456789', 480.00),
(5, 3, '2024-03-01 11:30:00', 'completed', '789 Park Avenue, District 2, Ho Chi Minh City', '0912345678', 115.00);

SET IDENTITY_INSERT orders OFF;
GO

-- =============================================
-- 9. ORDER ITEMS
-- =============================================
SET IDENTITY_INSERT order_items ON;

INSERT INTO order_items (id, order_id, product_id, variant_id, quantity, price) VALUES
-- Order 1: Wilson Pro Staff racket
(1, 1, 1, 1, 1, 250.00),
-- Order 2: Babolat racket + Nike shoes
(2, 2, 2, 4, 1, 220.00),
(3, 2, 6, 11, 1, 135.00),
-- Order 3: Head Speed racket
(4, 3, 3, 7, 1, 240.00),
-- Order 4: Wilson Blade + Nike Polo + Shorts
(5, 4, 4, NULL, 1, 153.00),
(6, 4, 10, 23, 2, 45.00),
(7, 4, 11, 27, 2, 31.50),
-- Order 5: Women shoes
(8, 5, 8, 17, 1, 115.00);

SET IDENTITY_INSERT order_items OFF;
GO

-- =============================================
-- 10. PAYMENTS
-- =============================================
SET IDENTITY_INSERT payments ON;

INSERT INTO payments (id, order_id, payment_method, payment_status, amount, paid_at) VALUES
(1, 1, 'Credit Card', 'paid', 250.00, '2024-02-25 10:35:00'),
(2, 2, 'Cash on Delivery', 'unpaid', 355.00, NULL),
(3, 3, 'Bank Transfer', 'paid', 240.00, '2024-02-27 09:20:00'),
(4, 4, 'Credit Card', 'unpaid', 480.00, NULL),
(5, 5, 'E-Wallet', 'paid', 115.00, '2024-03-01 11:35:00');

SET IDENTITY_INSERT payments OFF;
GO

-- =============================================
-- 11. REVIEWS
-- =============================================
SET IDENTITY_INSERT reviews ON;

INSERT INTO reviews (id, user_id, product_id, rating, comment, created_at) VALUES
(1, 2, 1, 5, 'Excellent racket! Great control and feel. Highly recommended for advanced players.', '2024-02-26 15:00:00'),
(2, 3, 2, 4, 'Very powerful racket. Takes some getting used to but worth it.', '2024-02-27 10:00:00'),
(3, 2, 6, 5, 'Comfortable shoes with great support. Perfect for long matches.', '2024-02-28 14:30:00'),
(4, 4, 4, 4, 'Good racket for the price. Good balance of control and power.', '2024-03-02 09:00:00'),
(5, 3, 8, 5, 'Love these shoes! Very comfortable and stylish.', '2024-03-03 16:20:00');

SET IDENTITY_INSERT reviews OFF;
GO

-- =============================================
-- 12. PROMO CODES
-- =============================================
SET IDENTITY_INSERT promo_codes ON;

INSERT INTO promo_codes (id, code, discount_percent, valid_from, valid_to, is_active) VALUES
(1, 'WELCOME10', 10.00, '2024-01-01', '2024-12-31', 1),
(2, 'TENNIS20', 20.00, '2024-03-01', '2024-03-31', 1),
(3, 'SUMMER15', 15.00, '2024-06-01', '2024-08-31', 1),
(4, 'NEWUSER', 25.00, '2024-01-01', '2024-12-31', 1),
(5, 'VIP30', 30.00, '2024-01-01', '2024-12-31', 0);

SET IDENTITY_INSERT promo_codes OFF;
GO

-- =============================================
-- 13. NEWSLETTER SUBSCRIBERS
-- =============================================
SET IDENTITY_INSERT newsletter_subscribers ON;

INSERT INTO newsletter_subscribers (id, email, subscribed_at) VALUES
(1, 'customer1@email.com', '2024-01-05 10:00:00'),
(2, 'customer2@email.com', '2024-01-10 14:30:00'),
(3, 'customer3@email.com', '2024-01-15 09:20:00'),
(4, 'customer4@email.com', '2024-02-01 16:45:00'),
(5, 'customer5@email.com', '2024-02-10 11:15:00'),
(6, 'john.doe@email.com', '2024-02-15 08:30:00'),
(7, 'jane.smith@email.com', '2024-02-20 13:00:00');

SET IDENTITY_INSERT newsletter_subscribers OFF;
GO

-- =============================================
-- VERIFICATION QUERIES
-- =============================================

-- Check counts
SELECT 'user_roles' as table_name, COUNT(*) as count FROM user_roles
UNION ALL
SELECT 'brands', COUNT(*) FROM brands
UNION ALL
SELECT 'product_categories', COUNT(*) FROM product_categories
UNION ALL
SELECT 'users', COUNT(*) FROM users
UNION ALL
SELECT 'products', COUNT(*) FROM products
UNION ALL
SELECT 'product_variants', COUNT(*) FROM product_variants
UNION ALL
SELECT 'product_images', COUNT(*) FROM product_images
UNION ALL
SELECT 'orders', COUNT(*) FROM orders
UNION ALL
SELECT 'order_items', COUNT(*) FROM order_items
UNION ALL
SELECT 'payments', COUNT(*) FROM payments
UNION ALL
SELECT 'reviews', COUNT(*) FROM reviews
UNION ALL
SELECT 'promo_codes', COUNT(*) FROM promo_codes
UNION ALL
SELECT 'newsletter_subscribers', COUNT(*) FROM newsletter_subscribers;

PRINT 'Sample data insertion completed successfully!';
GO



