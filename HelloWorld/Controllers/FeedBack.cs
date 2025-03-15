using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Rental.Controllers
{
    public class FeedBackController : BaseController
    {
        public FeedBackController(ClassLibrary.Persistence.DBContext context) : base(context) { }

        [HttpPost]
        public IActionResult Create(FeedBack feedback)
        {
            // 1️⃣ Ensure User is Logged In
            var user = GetUserObject(); // Get user from cookie
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to leave feedback.";
                return RedirectToAction("Details", "Equipment", new { id = feedback.Equipment });
            }

            // 2️⃣ Populate Required Fields
            feedback.UserId = user.Id; // Set the logged-in user ID
            feedback.Date = DateTime.Now;
            feedback.Time = TimeOnly.FromDateTime(DateTime.Now);

            // Ensure Equipment Exists
            var equipmentExists = _context.Equipment.Any(e => e.Id == feedback.Equipment);
            if (!equipmentExists)
            {
                TempData["ErrorMessage"] = "Invalid equipment selected.";
                return RedirectToAction("Index", "Equipment");
            }

            // 3️⃣ Save to Database
            _context.FeedBacks.Add(feedback);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Feedback submitted successfully!";
            return RedirectToAction("Details", "Equipment", new { id = feedback.Equipment });
        }
    }
}
