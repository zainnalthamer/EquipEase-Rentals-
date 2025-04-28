using ClassLibrary.Models;
using ClassLibrary.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rental.Controllers
{
    public class RentalRequestController : BaseController
    {
        public RentalRequestController(DBContext context) : base(context) { }

        public IActionResult Index()
        {
            int? userId = GetUserObject()?.Id;

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in.";
                return RedirectToAction("Index", "SignIn");
            }

            var rentalHistory = _context.RentalRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.Equipment)
                .Include(r => r.RentalStatus1)
                .ToList();

            return View(rentalHistory);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int equipmentId)
        {
            var user = GetUserObject();
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please log in first.";
                return RedirectToAction("Index", "SignIn");
            }

            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null)
            {
                TempData["ErrorMessage"] = "Equipment not found.";
                return RedirectToAction("Index", "Equipment");
            }

            var request = new RentalRequest
            {
                EquipmentId = equipment.Id,
                UserId = user.Id,
                StartDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(7),
                Cost = equipment.Price * 7,
                RentalStatus = 1 // Pending
            };

            try
            {
                await _context.RentalRequests.AddAsync(request);
                await _context.SaveChangesAsync();

                // ✅ Log action
                await SaveLogAsync("Create Rental Request", $"Request created for EquipmentID: {equipment.Id}", "Web");

                TempData["SuccessMessage"] = "Rental request submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"Error creating rental request: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int requestId)
        {
            var request = await _context.RentalRequests.FindAsync(requestId);
            if (request == null) return NotFound("Rental request not found.");

            request.RentalStatus = 2; // Approved
            await _context.SaveChangesAsync();

            // ✅ Log action
            await SaveLogAsync("Approve Rental Request", $"RequestID: {requestId} approved", "Web");

            TempData["SuccessMessage"] = "Rental request approved successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int requestId)
        {
            var request = await _context.RentalRequests.FindAsync(requestId);
            if (request == null) return NotFound("Rental request not found.");

            request.RentalStatus = 3; // Rejected
            await _context.SaveChangesAsync();

            // ✅ Log action
            await SaveLogAsync("Reject Rental Request", $"RequestID: {requestId} rejected", "Web");

            TempData["ErrorMessage"] = "Rental request has been rejected.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int requestId)
        {
            var request = await _context.RentalRequests.FindAsync(requestId);
            if (request == null) return NotFound("Rental request not found.");

            request.RentalStatus = 8; // Completed
            await _context.SaveChangesAsync();

            // ✅ Log action
            await SaveLogAsync("Complete Rental", $"Rental completed for RequestID: {requestId}", "Web");

            TempData["SuccessMessage"] = "Rental marked as completed!";
            return RedirectToAction("Index");
        }

        public IActionResult Payment(int equipmentId)
        {
            var equipment = _context.Equipment.FirstOrDefault(e => e.Id == equipmentId);
            if (equipment == null)
            {
                TempData["ErrorMessage"] = "Equipment not found.";
                return RedirectToAction("Index", "Equipment");
            }

            ViewBag.Equipment = equipment;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAfterPayment(int equipmentId)
        {
            var user = GetUserObject();
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please log in.";
                return RedirectToAction("Index", "SignIn");
            }

            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null)
            {
                TempData["ErrorMessage"] = "Equipment not found.";
                return RedirectToAction("Index", "Equipment");
            }

            var request = new RentalRequest
            {
                EquipmentId = equipment.Id,
                UserId = user.Id,
                StartDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(7),
                Cost = equipment.Price * 7,
                RentalStatus = 1 // Pending
            };

            await _context.RentalRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            // ✅ Log action
            await SaveLogAsync("Rental After Payment", $"Rental created for EquipmentID: {equipmentId}", "Web");

            TempData["SuccessMessage"] = "Payment successful. Rental request created!";
            return RedirectToAction("Index");
        }
    }
}
