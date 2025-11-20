using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TennisShop.Models;
using Services;
using BussinessObject;

namespace TennisShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IBlogService _blogService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IBlogService blogService)
        {
            _logger = logger;
            _productService = productService;
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var categories = await _productService.GetAllCategorysAsync();
            var blogPosts = await _blogService.GetRecentPostsAsync(3);
            
            ViewBag.Categories = categories;
            ViewBag.BlogPosts = blogPosts;
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
