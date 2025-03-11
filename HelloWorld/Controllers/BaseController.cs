using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HelloWorld.Controllers
{
    public class BaseController : Controller
    {
        internal readonly ClassLibrary.Persistence.DBContext? _context;

        // Constructor injection
        public BaseController(ClassLibrary.Persistence.DBContext context)
        {
            _context = context;
        }

        public bool IsAuthenticated()
        {
            // try getting the credentials cookie, if it exists it will return true to inform the method that the user is authenticated, otherwise return false
            if (!Request.Cookies.TryGetValue("credentials", out string? userCookie) || userCookie == null)
            {
                return false;
            }
            return true;
        }

        public User? GetUserObject()
        {
            // try getting the credentials cookie, if it exists it will return the user object, otherwise return null
            if (!Request.Cookies.TryGetValue("credentials", out string? userCookie) || userCookie == null)
            {
                return null;
            }
            var cookieValue = Encoding.UTF8.GetString(Convert.FromBase64String(userCookie));
            var user = JsonConvert.DeserializeObject<User>(cookieValue);
            return user;
        }
    }
}
