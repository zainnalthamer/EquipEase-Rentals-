using System.Diagnostics;
using System.Text;
using ClassLibrary.Models;
using HelloWorld.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HelloWorld.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger): base(null)
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
            if (!IsAuthenticated())
            {
                return Redirect("/SignIn");
            }

            try
            {
                User? user = GetUserObject();
                if (user == null)
                {
                    throw new Exception("User does not exist");
                }
            } catch (Exception e)
            {
                ViewBag.Error = e.Message;
            }
            return View(ViewBag);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
