using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
