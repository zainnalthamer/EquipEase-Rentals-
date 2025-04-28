using ClassLibrary.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models;
using System.Threading.Tasks;

namespace Rental.Controllers
{
    public class UserManagementController : BaseController
    {
        public UserManagementController(DBContext context) : base(context) { }

        public IActionResult ManageUsers()
        {
            var user = GetUserObject();
            if (user == null || user.RoleId != 1) return RedirectToAction("Index", "Home");

            var users = _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId != 1) // Exclude Admins from being listed
                .ToList();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(int id, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["Error"] = "New password cannot be empty.";
                return RedirectToAction("ManageUsers");
            }

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("ManageUsers");
                }

                user.Password = newPassword.Trim();
                await _context.SaveChangesAsync();

                // ✅ Save log after successful password update
                await SaveLogAsync("Update Password", $"Password updated for UserID: {user.Id}", "Web");

                TempData["Success"] = $"Password updated successfully for {user.Email}.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while updating the password.";
            }

            return RedirectToAction("ManageUsers");
        }
    }
}
