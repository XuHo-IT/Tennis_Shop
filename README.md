# Tennis Shop

A comprehensive tennis equipment e-commerce platform built with ASP.NET Core MVC.

## Features

### 🎾 **Product Management**
- Browse tennis equipment and accessories
- Product search and filtering
- Detailed product information with images
- Stock management

### 👥 **User Management**
- User registration and authentication
- Role-based access control (Admin/Customer)
- User profile management
- Secure password handling

### 🔐 **Admin Dashboard**
- Complete admin panel for managing users and products
- User statistics and analytics
- Product management (create, edit, delete)
- Role management (promote users to admin)
- Access control - only admins can edit products

### 🛒 **E-commerce Features**
- Product catalog with categories
- Image upload and management (ImageKit integration)
- Shopping cart functionality
- Order management system

## Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: Cookie-based authentication
- **Image Storage**: ImageKit integration
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript

## Project Structure

```
TennisShop/
├── BussinessObject/          # Data models and entities
├── DataAccessLayer/          # Database context and DAOs
├── Repositories/             # Repository pattern implementation
├── Services/                 # Business logic services
├── TennisShop/              # Main web application
│   ├── Controllers/          # MVC controllers
│   ├── Views/               # Razor views
│   ├── Models/              # View models
│   └── wwwroot/             # Static files
└── TennisShop.sln           # Solution file
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/XuHo-IT/Tennis_Shop.git
   cd Tennis_Shop
   ```

2. **Configure Database**
   - Update connection string in `appsettings.json`
   - Run Entity Framework migrations (if needed)

3. **Initialize Database**
   - Run the application
   - Visit the home page
   - Click "Setup Database" to initialize roles and admin account
   - Default admin credentials: `admin@tennisshop.com` / `admin123`

4. **Run the Application**
   ```bash
   dotnet run
   ```
   - Navigate to `https://localhost:5001`

## Default Accounts

### Admin Account
- **Email**: admin@tennisshop.com
- **Password**: admin123
- **Access**: Full admin dashboard, user management, product management

### Customer Account
- Register a new account or use existing customer accounts
- **Access**: View products, browse catalog

## Features Overview

### For Administrators
- 📊 **Dashboard**: View statistics (users, products, orders)
- 👥 **User Management**: Edit user details, change roles, delete users
- 📦 **Product Management**: Create, edit, delete products
- 🔧 **System Administration**: Manage roles and permissions

### For Customers
- 🛍️ **Product Browsing**: View and search products
- 🔍 **Product Details**: Detailed product information
- 👤 **User Profile**: Manage personal information
- 🛒 **Shopping**: Add products to cart (future feature)

## Database Schema

### Key Tables
- **Users**: User accounts with role-based access
- **UserRoles**: Role definitions (admin, customer)
- **Products**: Product catalog
- **ProductImages**: Product image management
- **Orders**: Order management (future feature)

## Security Features

- ✅ Role-based authorization
- ✅ Secure password hashing
- ✅ Cookie-based authentication
- ✅ Admin-only access to sensitive operations
- ✅ Input validation and sanitization

## API Endpoints

### Admin Endpoints (Admin Only)
- `/Admin/Dashboard` - Admin dashboard
- `/Admin/Users` - User management
- `/Admin/Products` - Product management
- `/Admin/Debug` - Debug information

### Product Endpoints
- `/Product/Index` - Product catalog
- `/Product/Details/{id}` - Product details
- `/Product/Create` - Create product (Admin only)
- `/Product/Edit/{id}` - Edit product (Admin only)
- `/Product/Delete/{id}` - Delete product (Admin only)

### Account Endpoints
- `/Account/Login` - User login
- `/Account/Register` - User registration
- `/Account/Profile` - User profile
- `/Account/Logout` - User logout

## Development

### Adding New Features
1. Create models in `BussinessObject/`
2. Add data access in `DataAccessLayer/`
3. Implement business logic in `Services/`
4. Create controllers in `TennisShop/Controllers/`
5. Add views in `TennisShop/Views/`

### Database Changes
1. Update models in `BussinessObject/`
2. Create migration: `dotnet ef migrations add MigrationName`
3. Update database: `dotnet ef database update`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

- **Developer**: Xuan Hoa
- **Email**: ngotranxuanhoa09062004@gmail.com
- **GitHub**: [XuHo-IT](https://github.com/XuHo-IT)

## Acknowledgments

- Built with ASP.NET Core MVC
- Bootstrap for responsive design
- Entity Framework Core for data access
- ImageKit for image management
