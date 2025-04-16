using ClassLibrary.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Rental.Controllers
{
    public class AdminController : BaseController
    {
        public AdminController(DBContext context) : base(context) { }

        public IActionResult Dashboard()
        {
            var user = GetUserObject();
            if (user == null || user.RoleId != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.TotalManagers = _context.Users.Count(u => u.RoleId == 1);
            ViewBag.TotalCustomers = _context.Users.Count(u => u.RoleId == 3);
            ViewBag.TotalEquipment = _context.Equipment.Count();
            ViewBag.AvailableEquipment = _context.Equipment.Count(e => e.AvailableId == 1);
            ViewBag.UnavailableEquipment = _context.Equipment.Count(e => e.AvailableId != 1);
            ViewBag.RentalRequests = _context.RentalRequests.Count();

            return View();
        }

    }
}
