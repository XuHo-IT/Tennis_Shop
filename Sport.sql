CREATE DATABASE Sport;
GO

USE Sport;
GO

-- =========================
-- 1️⃣ USERS & ROLES
-- =========================
CREATE TABLE user_roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL
);
GO

CREATE TABLE users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(120) UNIQUE NOT NULL,
    password_hash VARCHAR(MAX) NOT NULL,
    phone VARCHAR(20),
    address VARCHAR(MAX),
    role_id INT,
    created_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_users_roles FOREIGN KEY (role_id) REFERENCES user_roles(id)
);
GO

-- =========================
-- 2️⃣ PRODUCT CATEGORIES & BRANDS
-- =========================
CREATE TABLE brands (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description VARCHAR(MAX)
);
GO

CREATE TABLE product_categories (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(MAX),
    parent_id INT NULL,
    CONSTRAINT FK_category_parent FOREIGN KEY (parent_id) REFERENCES product_categories(id)
);
GO

-- =========================
-- 3️⃣ PRODUCTS & INVENTORY
-- =========================
CREATE TABLE products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(MAX),
    category_id INT,
    brand_id INT,
    base_price DECIMAL(10,2) NOT NULL,
    discount_percent DECIMAL(5,2) DEFAULT 0,
    stock INT DEFAULT 0,
    is_active BIT DEFAULT 1,
    created_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_products_category FOREIGN KEY (category_id) REFERENCES product_categories(id),
    CONSTRAINT FK_products_brand FOREIGN KEY (brand_id) REFERENCES brands(id)
);
GO

CREATE TABLE product_variants (
    id INT IDENTITY(1,1) PRIMARY KEY,
    product_id INT NOT NULL,
    color VARCHAR(50),
    size VARCHAR(20),
    sku VARCHAR(100) UNIQUE,
    stock INT DEFAULT 0,
    price DECIMAL(10,2),
    CONSTRAINT FK_variant_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
);
GO

CREATE TABLE product_images (
    id INT IDENTITY(1,1) PRIMARY KEY,
    product_id INT NOT NULL,
    image_url VARCHAR(MAX) NOT NULL,
    image_id VARCHAR(100),
    is_main BIT,
    is_primary BIT,
    CONSTRAINT FK_image_product FOREIGN KEY (product_id) REFERENCES products(id) ON DELETE CASCADE
);
GO

-- =========================
-- 4️⃣ ORDERS & PAYMENTS
-- =========================
CREATE TABLE orders (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    order_date DATETIME DEFAULT GETDATE(),
    status VARCHAR(30) DEFAULT 'pending',
    shipping_address VARCHAR(MAX),
    phone VARCHAR(20),
    total_amount DECIMAL(10,2),
    CONSTRAINT FK_orders_user FOREIGN KEY (user_id) REFERENCES users(id)
);
GO

CREATE TABLE order_items (
    id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT NOT NULL,
    product_id INT,
    variant_id INT,
    quantity INT NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_orderitem_order FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    CONSTRAINT FK_orderitem_product FOREIGN KEY (product_id) REFERENCES products(id),
    CONSTRAINT FK_orderitem_variant FOREIGN KEY (variant_id) REFERENCES product_variants(id)
);
GO

CREATE TABLE payments (
    id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT NOT NULL,
    payment_method VARCHAR(50),
    payment_status VARCHAR(30) DEFAULT 'unpaid',
    amount DECIMAL(10,2),
    paid_at DATETIME,
    CONSTRAINT FK_payment_order FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE
);
GO

-- =========================
-- 5️⃣ REVIEWS, WISHLIST, PROMOS
-- =========================
CREATE TABLE reviews (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT,
    product_id INT,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    comment VARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_review_user FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT FK_review_product FOREIGN KEY (product_id) REFERENCES products(id)
);
GO

CREATE TABLE promo_codes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    discount_percent DECIMAL(5,2) NOT NULL,
    valid_from DATE,
    valid_to DATE,
    is_active BIT DEFAULT 1
);
GO

CREATE TABLE newsletter_subscribers (
    id INT IDENTITY(1,1) PRIMARY KEY,
    email VARCHAR(120) UNIQUE NOT NULL,
    subscribed_at DATETIME DEFAULT GETDATE()
);
GO

    -- =========================
    -- 6️⃣ CART 
    -- =========================
    CREATE TABLE carts (
        id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NULL,
        created_at DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_carts_user FOREIGN KEY (user_id) REFERENCES users(id)
    );
    GO

    CREATE TABLE cart_items (
        id INT IDENTITY(1,1) PRIMARY KEY,
        cart_id INT NOT NULL,
        product_id INT NOT NULL,
        variant_id INT NULL,                     
        quantity INT NOT NULL DEFAULT 1,
        unit_price DECIMAL(10,2) NOT NULL,

        CONSTRAINT FK_cartitems_cart FOREIGN KEY (cart_id) REFERENCES carts(id) ON DELETE CASCADE,
        CONSTRAINT FK_cartitems_product FOREIGN KEY (product_id) REFERENCES products(id),
        CONSTRAINT FK_cartitems_variant FOREIGN KEY (variant_id) REFERENCES product_variants(id) ON DELETE SET NULL, -- ✅ Thêm quan hệ variant
        CONSTRAINT CK_cartitems_quantity CHECK (quantity > 0)
    );
    GO

    CREATE UNIQUE INDEX UQ_cart_variant_per_cart ON cart_items (cart_id, variant_id)
    WHERE variant_id IS NOT NULL;
    GO

    CREATE UNIQUE INDEX UQ_cart_product_per_cart ON cart_items (cart_id, product_id)
    WHERE variant_id IS NULL AND product_id IS NOT NULL;
    GO




-- ===================================
-- 2️⃣ BRANDS & CATEGORIES
-- ===================================
INSERT INTO brands (name, description)
VALUES
('Wilson', 'Top tennis equipment brand from the USA'),
('Head', 'Austrian brand known for rackets and balls'),
('Babolat', 'French company specializing in tennis gear'),
('Yonex', 'Japanese brand for rackets and strings');
GO

INSERT INTO product_categories (name, description, parent_id)
VALUES
('Rackets', 'All types of tennis rackets', NULL),
('Balls', 'Official tennis balls and training balls', NULL),
('Shoes', 'Tennis shoes for men and women', NULL),
('Apparel', 'Clothing and accessories for tennis players', NULL),
('Bags', 'Racket and gear bags', NULL);
GO



-- ===================================
-- 3️⃣ PRODUCTS
-- ===================================
INSERT INTO products (name, description, category_id, brand_id, base_price, discount_percent, stock)
VALUES
('Wilson Pro Staff 97', 'Classic control-oriented racket used by Roger Federer', 1, 1, 299.99, 10, 15),
('Head Speed MP 2024', 'Balanced racket with power and control, used by Novak Djokovic', 1, 2, 259.99, 5, 20),
('Babolat Pure Drive', 'Powerful and versatile racket loved by aggressive players', 1, 3, 279.99, 8, 18),
('Yonex Ezone 98', 'Comfort and precision from the Japanese engineering', 1, 4, 285.00, 0, 10),

('Wilson US Open Tennis Balls (3 Pack)', 'Official US Open tennis balls', 2, 1, 8.99, 0, 100),
('Head Tour XT Balls (4 Pack)', 'Premium pressurized tennis balls for tournaments', 2, 2, 10.99, 0, 80),

('Babolat Jet Mach 3 Men Shoes', 'Lightweight and durable shoes for men', 3, 3, 149.99, 15, 25),
('Yonex Fusionrev 5 Women Shoes', 'High-grip women’s tennis shoes with cushioning', 3, 4, 139.99, 10, 20),

('Wilson Team Backpack', 'Spacious tennis backpack for 2 rackets', 5, 1, 69.99, 5, 30);
GO



-- ===================================
-- 3️⃣ PRODUCT VARIANTS
-- ===================================
INSERT INTO product_variants (product_id, color, size, sku, stock, price)
VALUES
(1, 'Black/Red', 'Standard', 'WIL-PS97-BR', 10, 299.99),
(2, 'White/Black', 'Standard', 'HEA-SPMP-WB', 12, 259.99),
(3, 'Blue', 'Standard', 'BAB-PD-BL', 8, 279.99),
(4, 'Blue/Navy', 'Standard', 'YON-EZ98-BN', 5, 285.00),

(7, 'Blue', 'US 9', 'BAB-JM3-9', 5, 149.99),
(7, 'Blue', 'US 10', 'BAB-JM3-10', 7, 149.99),
(8, 'Pink', 'US 7', 'YON-FR5-7', 4, 139.99),
(8, 'Pink', 'US 8', 'YON-FR5-8', 6, 139.99);
GO


-- ===================================
-- 3️⃣ PRODUCT IMAGES
-- link imagekit đang sai
-- ===================================
INSERT INTO product_images (product_id, image_url, image_id, is_main, is_primary)
VALUES
(1, 'https://ik.imagekit.io/tennisshop/wilson_pro_staff_97.jpg', 'wilson_pro_staff_97', 1, 1),
(2, 'https://ik.imagekit.io/tennisshop/head_speed_mp.jpg', 'head_speed_mp', 1, 1),
(3, 'https://ik.imagekit.io/tennisshop/babolat_pure_drive.jpg', 'babolat_pure_drive', 1, 1),
(4, 'https://ik.imagekit.io/tennisshop/yonex_ezone_98.jpg', 'yonex_ezone_98', 1, 1),
(5, 'https://ik.imagekit.io/tennisshop/wilson_usopen_balls.jpg', 'wilson_usopen_balls', 1, 1),
(7, 'https://ik.imagekit.io/tennisshop/babolat_jet_mach_3.jpg', 'babolat_jet_mach_3', 1, 1),
(9, 'https://ik.imagekit.io/tennisshop/wilson_team_backpack.jpg', 'wilson_team_backpack', 1, 1);
GO

-- ===================================
-- 4️⃣ PROMO & NEWSLETTER
-- ===================================
INSERT INTO promo_codes (code, discount_percent, valid_from, valid_to)
VALUES
('WELCOME10', 10, '2025-01-01', '2025-12-31'),
('SUMMER15', 15, '2025-06-01', '2025-09-01'),
('FREESHIP', 0, '2025-01-01', '2025-12-31');
GO

INSERT INTO newsletter_subscribers (email)
VALUES
('fan.tennis@gmail.com'),
('sportlover@yahoo.com');
GO

