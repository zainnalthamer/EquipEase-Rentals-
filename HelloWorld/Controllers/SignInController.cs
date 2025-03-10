using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ClassLibrary.Models;

namespace HelloWorld.Controllers
{
    public class SignInController : Controller
    {
        private readonly ClassLibrary.Persistence.DBContext _context;
        public SignInController(ClassLibrary.Persistence.DBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SignIn(string username, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == username && u.Password == password);

            if (user != null)
            {
                ViewBag.Message = "Welcome! You have successfully signed in.";
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
            }

            return View("Index");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
