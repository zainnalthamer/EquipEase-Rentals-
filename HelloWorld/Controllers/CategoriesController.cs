using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    public class CategoriesController : BaseController
    {

        public CategoriesController(ClassLibrary.Persistence.DBContext context): base(context)
        {

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetCategories()
        {
            try
            {
                IEnumerable<ClassLibrary.Models.Category> categories = _context.Categories; 
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
            }

            return View(ViewBag);
        }
    }
}
