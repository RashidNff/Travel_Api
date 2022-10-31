using System;

namespace TRAVEL_CORE.Entities.Order
{
    public class SaveOrder
    {
        public int Id { get; set; }
        public string? OrderNo { get; set; }
        public DateTime Orderdate { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int CreatedBy { get; set; }
        public Airway? AirwayData { get; set; }
        public Hotel? HotelData { get; set; }

    }
}
