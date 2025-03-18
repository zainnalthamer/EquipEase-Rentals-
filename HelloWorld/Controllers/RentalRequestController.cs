using ClassLibrary.Models;
using ClassLibrary.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rental.Controllers
{
    public class RentalRequestController : Controller
    {
        private readonly DBContext _context;

        public RentalRequestController(DBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); 
        }

        // List all rental requests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.RentalRequests
                .Include(r => r.Equipment)
                .Include(r => r.RentalStatus1) // Include the RentalStatus entity
                .ToListAsync();

            return View("~/Views/RentalRequest/Index.cshtml", requests);
        }

        // Create a new rental request from equipment details page
        [HttpPost]
        public async Task<IActionResult> Create(int equipmentId)
        {
            if (_context == null)
            {
                return View("Error", "Database context is not available.");
            }

            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null)
                return NotFound("Equipment not found.");

            var request = new RentalRequest
            {
                EquipmentId = equipment.Id,
                StartDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddDays(7),
                Cost = equipment.Price * 7,
                RentalStatus = 1 
            };

            try
            {
                await _context.RentalRequests.AddAsync(request);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Rental request submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                return View("Error", $"Database error: {ex.InnerException?.Message}");
            }
        }

        // Approve Rental Request
        [HttpPost]
        public async Task<IActionResult> Approve(int requestId)
        {
            var request = await _context.RentalRequests.FindAsync(requestId);
            if (request == null) return NotFound("Rental request not found.");

            request.RentalStatus = 2; // Approved
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rental request approved successfully!";
            return RedirectToAction("Index");
        }

        // Reject Rental Request
        [HttpPost]
        public async Task<IActionResult> Reject(int requestId)
        {
            var request = await _context.RentalRequests.FindAsync(requestId);
            if (request == null) return NotFound("Rental request not found.");

            request.RentalStatus = 3; // Rejected
            await _context.SaveChangesAsync();

            TempData["ErrorMessage"] = "Rental request has been rejected.";
            return RedirectToAction("Index");
        }

        // Mark Rental as Completed
        [HttpPost]
        public async Task<IActionResult> Complete(int requestId)
        {
            var request = await _context.RentalRequests.FindAsync(requestId);
            if (request == null) return NotFound("Rental request not found.");

            request.RentalStatus = 8; // Completed
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rental marked as completed!";
            return RedirectToAction("Index");
        }
    }
}
