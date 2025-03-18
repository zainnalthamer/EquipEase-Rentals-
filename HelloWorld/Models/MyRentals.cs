namespace Rental.Models
{
    public class MyRentals
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; }
        public string RentalStatus { get; set; }
        public decimal Cost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }

        public MyRentals(int id, string equipmentName, string rentalStatus, decimal cost, DateTime startDate, DateTime endDate, int userId)
        {
            Id = id;
            EquipmentName = equipmentName;
            RentalStatus = rentalStatus;
            Cost = cost;
            StartDate = startDate;
            EndDate = endDate;
            UserId = userId;
        }
    }
}
