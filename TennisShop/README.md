# TennisShop - CRUD Product Management with Authentication

This is a complete ASP.NET Core MVC application implementing a tennis shop with product CRUD operations and user authentication.

## Architecture

The application follows a layered architecture pattern:

### 1. Data Access Layer (DAL)
- **SportManagementContext**: Entity Framework DbContext
- **ProductDAO**: Data Access Object for Product operations
- **UserDAO**: Data Access Object for User operations with password hashing

### 2. Repository Layer
- **IProductRepository & ProductRepository**: Repository pattern for Product data access
- **IUserRepository & UserRepository**: Repository pattern for User data access

### 3. Service Layer
- **IProductService & ProductService**: Business logic for Product operations
- **IUserService & UserService**: Business logic for User operations with validation

### 4. MVC Layer
- **ProductController**: Handles Product CRUD operations with authorization
- **AccountController**: Handles user authentication (Login/Register/Profile)
- **HomeController**: Displays featured products on homepage

## Features

### Product Management
- ✅ View all products (public)
- ✅ View product details (public)
- ✅ Create new products (authenticated users only)
- ✅ Edit products (authenticated users only)
- ✅ Delete products (authenticated users only)
- ✅ Search products
- ✅ Filter by category

### User Authentication
- ✅ User registration
- ✅ User login with email/password
- ✅ User profile management
- ✅ Secure password hashing (SHA256)
- ✅ Cookie-based authentication
- ✅ Logout functionality

### UI Features
- ✅ Responsive Bootstrap design
- ✅ Font Awesome icons
- ✅ Product cards with images
- ✅ Search functionality
- ✅ Navigation with authentication status
- ✅ User dropdown menu

## Database Connection

The application uses SQL Server with the following connection string:
```
Server=localhost\SQLEXPRESS;Database=Sport;User ID=sa;Password=sa123456;TrustServerCertificate=True
```

## Getting Started

1. **Prerequisites**:
   - .NET 8.0 SDK
   - SQL Server (LocalDB or Express)
   - Visual Studio 2022 or VS Code

2. **Database Setup**:
   - Ensure SQL Server is running
   - The database "Sport" should exist with the required tables
   - Update connection string in `appsettings.json` if needed

3. **Run the Application**:
   ```bash
   cd TennisShop
   dotnet run
   ```

4. **Access the Application**:
   - Navigate to `https://localhost:5001` or `http://localhost:5000`
   - Register a new user account
   - Login to access product management features

## Project Structure

```
TennisShop/
├── Controllers/
│   ├── HomeController.cs
│   ├── ProductController.cs
│   └── AccountController.cs
├── Views/
│   ├── Home/
│   ├── Product/
│   └── Account/
├── DataAccessLayer/
│   ├── SportManagementContext.cs
│   ├── ProductDAO.cs
│   └── UserDAO.cs
├── Repositories/
│   ├── IProductRepository.cs
│   ├── ProductRepository.cs
│   ├── IUserRepository.cs
│   └── UserRepository.cs
├── Services/
│   ├── IProductService.cs
│   ├── ProductService.cs
│   ├── IUserService.cs
│   └── UserService.cs
└── BussinessObject/
    ├── Product.cs
    ├── User.cs
    └── ... (other entities)
```

## Security Features

- Password hashing using SHA256
- Cookie-based authentication
- Authorization attributes on sensitive operations
- CSRF protection with anti-forgery tokens
- Input validation and sanitization

## Dependencies

- Microsoft.EntityFrameworkCore.SqlServer (8.0.2)
- Microsoft.AspNetCore.Authentication.Cookies (2.2.0)
- Bootstrap 5 for UI
- Font Awesome for icons

## Usage

1. **Public Access**: Anyone can view products and product details
2. **User Registration**: New users can register with email/password
3. **User Login**: Registered users can login to access additional features
4. **Product Management**: Authenticated users can create, edit, and delete products
5. **Profile Management**: Users can view and edit their profile information

## Future Enhancements

- Role-based authorization (Admin vs Regular User)
- Product image upload functionality
- Shopping cart functionality
- Order management
- Product categories and brands management
- Email verification for registration
- Password reset functionality
