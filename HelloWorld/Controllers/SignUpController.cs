using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    public class SignUpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
