using Microsoft.AspNetCore.Mvc;

namespace TennisShop.Controllers
{
    public class SharedController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
