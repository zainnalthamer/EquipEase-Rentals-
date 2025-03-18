using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Rental.Controllers
{
    public class MyRentals : BaseController
    {
        public MyRentals(ClassLibrary.Persistence.DBContext context) : base(context) { }
        public IActionResult Index()
        {
            int? userId = GetUserObject()?.Id;

           var rentalHistory = base._context.RentalRequests.Where(r => r.UserId == userId)
                .Include(r => r.Equipment)
                .Include(r => r.RentalStatus1)
                .Select(r => new Models.MyRentals(
                
                    r.Id,
                    r.Equipment.Name,
                    r.RentalStatus1.Status,  // Assuming RentalStatus1 has a StatusName property
                    r.Cost,               // Convert decimal to float
                    r.StartDate,
                    r.ReturnDate,
                    r.UserId
                ))
                .ToList();

            
            return Json(rentalHistory);
        }
    }
}
