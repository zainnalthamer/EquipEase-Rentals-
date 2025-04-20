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
        private readonly DBContext _context;

        public RentalRequestController(DBContext context) : base(context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? userId = GetUserObject()?.Id;

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
            if (_context == null)
            {
                TempData["ErrorMessage"] = "Database context is not available.";
                return RedirectToAction("Index", "MyRentals");
            }

            var user = GetUserObject();
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not authenticated.";
                return RedirectToAction("Index", "SignIn");
            }

            var equipment = await _context.Equipment.FindAsync(equipmentId);
            if (equipment == null)
            {
                TempData["ErrorMessage"] = "Equipment not found.";
                return RedirectToAction("Index", "MyRentals");
            }

            var request = new RentalRequest
            {
                EquipmentId = equipment.Id,
                UserId = user.Id,
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
                return RedirectToAction("Index", "MyRentals");
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"Error while creating rental request: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction("Index", "MyRentals");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int requestId)
        {
            var request = await _context.RentalRequests.FindAsync(requestId);
            if (request == null) return NotFound("Rental request not found.");

            request.RentalStatus = 2;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rental request approved successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int requestId)
        {
            var request = await _context.RentalRequests.FindAsync(requestId);
            if (request == null) return NotFound("Rental request not found.");

            request.RentalStatus = 3;
            await _context.SaveChangesAsync();

            TempData["ErrorMessage"] = "Rental request has been rejected.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int requestId)
        {
            var request = await _context.RentalRequests.FindAsync(requestId);
            if (request == null) return NotFound("Rental request not found.");

            request.RentalStatus = 8;
            await _context.SaveChangesAsync();

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
            return View(); // This will be Views/RentalRequest/Payment.cshtml
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
                RentalStatus = 1
            };

            await _context.RentalRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Payment successful. Rental request created!";
            return RedirectToAction("Index");
        }


    }
}