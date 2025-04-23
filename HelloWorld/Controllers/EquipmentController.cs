using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Persistence;
using ClassLibrary.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rental.Controllers
{
    public class EquipmentController : BaseController
    {
        public EquipmentController(DBContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            if (!IsAuthenticated())
            {
                ViewBag.IsAuthenticated = false;
                return Redirect("/SignIn");
            }

            ViewBag.IsAuthenticated = true;
            try
            {
                var equipmentQuery = _context.Equipment.AsQueryable();

                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    equipmentQuery = equipmentQuery.Where(e => e.CategoryId == categoryId.Value);
                }

                var equipmentList = await equipmentQuery
                    .Select(e => new
                    {
                        e.Id,
                        e.Name,
                        e.Description,
                        e.CategoryId,
                        e.Price,
                        e.AvailableId,
                        e.ConditionId,
                        ImageUrl = e.Image != null ? Convert.ToBase64String(e.Image) : null,
                        CategoryName = e.Category.Name
                    })
                    .ToListAsync();

                ViewBag.EquipmentList = equipmentList;
                //ViewBag.User=await _context.Users.User
                //var userEmail = User.Identity.Name; // This retrieves the email of the logged-in user
                //ViewBag.User = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == userEmail);
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.ConditionStatuses = await _context.ConditionStatuses.ToListAsync();
                ViewBag.AvailabilityStatuses = await _context.AvailableStatuses.ToListAsync();
                //var userEmail = User.Identity.Name;

                var user = GetUserObject();
                if (user != null)
                {
                    ViewBag.User = user;
                }
                else
                {
                    ViewBag.User = null;
                }

            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEquipment(string name, string description, int categoryId, decimal price, int availableId, int conditionId, IFormFile image)
        {
            if (!IsAuthenticated())
            {
                return Redirect("/SignIn");
            }

            byte[]? imageData = null;

            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            var newEquipment = new Equipment
            {
                Name = name,
                Description = description,
                CategoryId = categoryId,
                Price = price,
                AvailableId = availableId,
                ConditionId = conditionId,
                Image = imageData
            };

            _context.Equipment.Add(newEquipment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            if (!IsAuthenticated())
            {
                return Redirect("/SignIn");
            }

            var equipment = await _context.Equipment
                .Include(e => e.FeedBacks)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
            {
                return NotFound();
            }

            // ✅ NEW: Block delete if there are rental requests
            var hasRentalRequests = _context.RentalRequests.Any(r => r.EquipmentId == id);
            if (hasRentalRequests)
            {
                TempData["ErrorMessage"] = "Cannot delete equipment. It has related rental requests.";
                return RedirectToAction("Index");
            }

            // Remove related feedbacks
            if (equipment.FeedBacks.Any())
            {
                _context.FeedBacks.RemoveRange(equipment.FeedBacks);
            }

            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> EditEquipment(int id)
        {
            if (!IsAuthenticated())
            {
                return Redirect("/SignIn");
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.ConditionStatuses = await _context.ConditionStatuses.ToListAsync();
            ViewBag.AvailabilityStatuses = await _context.AvailableStatuses.ToListAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> EditEquipment(int id, string name, string description, int categoryId, decimal price, int availableId, int conditionId, IFormFile? image)
        {
            if (!IsAuthenticated())
            {
                return Redirect("/SignIn");
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            equipment.Name = name;
            equipment.Description = description;
            equipment.CategoryId = categoryId;
            equipment.Price = price;
            equipment.AvailableId = availableId;
            equipment.ConditionId = conditionId;

            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    equipment.Image = memoryStream.ToArray();
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(int id)
        {
            if (!IsAuthenticated())
            {
                ViewBag.IsAuthenticated = false;
                return Redirect("/SignIn");
            }

            ViewBag.IsAuthenticated = true;

            try
            {
                var equipment = await _context.Equipment
                    .Include(e => e.Category)
                    .Include(e => e.Available)
                    .Include(e => e.Condition)
                    .Include(e => e.FeedBacks) // Include feedbacks
                    .ThenInclude(f => f.User)  // Include users who left feedback
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (equipment == null)
                {
                    return NotFound();
                }

                ViewBag.FeedbackList = equipment.FeedBacks.ToList(); // Pass feedback to the view

                var user = GetUserObject();
                ViewBag.User = user ?? null;

                return View(equipment);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View("Error");
            }
        }

    }
}
