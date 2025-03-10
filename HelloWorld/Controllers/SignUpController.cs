using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloWorld.Controllers
{
    public class SignUpController : Controller
    {
        private readonly ClassLibrary.Persistence.DBContext _context;

        public SignUpController(ClassLibrary.Persistence.DBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CheckAccount()
        {
            IEnumerable<ClassLibrary.Models.User> users = _context.Users;
            return Json(users);
        }

        [HttpPost]
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateAccount(string firstName, string lastName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match!";
                return View("Index");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email already in use!";
                return View("Index");
            }

            var user = new User
            {
                Fname = firstName,
                Lname = lastName,
                Email = email,
                Password = password, 
                RoleId = 3 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Account successfully created! You can now sign in.";

            return RedirectToAction("Index", "SignIn");
        }
    }
}
