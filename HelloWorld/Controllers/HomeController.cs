using System.Diagnostics;
using System.Text;
using ClassLibrary.Models;
using HelloWorld.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HelloWorld.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Token()
        {
            if (!Request.Cookies.TryGetValue("credentials", out string? userCookie) || userCookie == null)
            {
                return Redirect("/SignIn");
            }

            var cookieValue = Encoding.UTF8.GetString(Convert.FromBase64String(userCookie));
            var user = JsonConvert.DeserializeObject<User>(cookieValue);
            return View(user);
             
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
