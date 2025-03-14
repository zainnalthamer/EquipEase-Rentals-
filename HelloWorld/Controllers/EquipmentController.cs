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
                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.ConditionStatuses = await _context.ConditionStatuses.ToListAsync();
                ViewBag.AvailabilityStatuses = await _context.AvailableStatuses.ToListAsync();
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
    }
}
