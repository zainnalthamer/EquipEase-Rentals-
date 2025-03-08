using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    public class SignInController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
