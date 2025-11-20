using BussinessObject;
using Repositories;
using Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework
builder.Services.AddDbContext<SportContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"), 
        b => b.MigrationsAssembly("DataAccessLayer")));

// --- Authentication: Cookie + Google ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Cookie mặc định
    options.DefaultChallengeScheme = "Google"; // Google login challenge
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Shared/AccessDenied";
})
.AddGoogle("Google", options =>
{
    // Đảm bảo giá trị ClientId và ClientSecret đúng trong appsettings.json
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
});

// --- Register DAOs ---
builder.Services.AddScoped<ProductDAO>();
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<OrderDAO>();
builder.Services.AddScoped<CartDAO>();

// --- Register Repositories ---
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();

// --- Register Services ---
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IImageKitService, ImageKitService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Services.AddSingleton<IBlogService, BlogService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();
builder.Services.AddHttpClient<IImageKitService, ImageKitService>();
builder.Services.AddHttpClient<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
builder.Services.AddHttpClient<IChatbotService, ChatbotService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- Authentication & Authorization ---
app.UseAuthentication();
app.UseAuthorization();

// --- Routing ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
