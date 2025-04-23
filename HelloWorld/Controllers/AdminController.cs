using ClassLibrary.Models;
using ClassLibrary.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            ViewBag.Categories = _context.Categories.ToList();

            return View();
        }
        [HttpPost]
        public IActionResult AddCategory(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return RedirectToAction("Dashboard");

            if (_context.Categories.Any(c => c.Name.ToLower() == name.ToLower()))
            {
                TempData["Error"] = "Category already exists.";
                return RedirectToAction("Dashboard");
            }

            _context.Categories.Add(new Category { Name = name.Trim() });
            _context.SaveChanges();

            TempData["Success"] = "Category added successfully!";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction("Dashboard");
            }

            bool hasEquipment = _context.Equipment.Any(e => e.CategoryId == id);
            bool hasRentalRequests = _context.RentalRequests
                .Include(r => r.Equipment)
                .Any(r => r.Equipment.CategoryId == id);

            if (hasEquipment || hasRentalRequests)
            {
                TempData["Error"] = "Cannot delete category in use.";
                return RedirectToAction("Dashboard");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            TempData["Success"] = "Category deleted successfully.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult UpdateCategory(int id, string newName)
        {
            var category = _context.Categories.Find(id);
            if (category == null || string.IsNullOrWhiteSpace(newName))
            {
                TempData["Error"] = "Invalid update.";
                return RedirectToAction("Dashboard");
            }

            category.Name = newName.Trim();
            _context.SaveChanges();

            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction("Dashboard");
        }


    }
}
