using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    public class HelloWorldController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Welcome(string name, int ID = 1, int repeat=1)
        {
            string retMessage="";
            retMessage += "A special welcome.."+name+".";
            retMessage += "Your ID is: " + ID.ToString()+". ";
            //retMessage += "Repeat: " + repeat + " times in this page!";
            ViewBag.Message = retMessage;
            ViewBag.Repeat = repeat;
            return View(ViewBag);
        }
    }
}
