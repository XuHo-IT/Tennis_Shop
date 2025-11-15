using Microsoft.AspNetCore.Mvc;
using Services;

namespace TennisShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // GET: Blog
        public async Task<IActionResult> Index()
        {
            var posts = await _blogService.GetAllPostsAsync();
            return View(posts);
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Increment views (in a real app, this would update the database)
            post.Views = (post.Views ?? 0) + 1;

            // Get related posts
            var allPosts = await _blogService.GetAllPostsAsync();
            var relatedPosts = allPosts
                .Where(p => p.Id != id && p.Category == post.Category)
                .Take(3)
                .ToList();

            ViewBag.RelatedPosts = relatedPosts;
            return View(post);
        }
    }
}
