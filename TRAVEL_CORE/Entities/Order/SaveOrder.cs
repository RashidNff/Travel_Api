using System;
using System.Text.Json.Serialization;

namespace TRAVEL_CORE.Entities.Order
{
    public class SaveOrder
    {
        public int Id { get; set; }
        public string? OrderNo { get; set; }
        public int? OrderType { get; set; }
        public DateTime OrderDate { get; set; }
        public int? CompanyId { get; set; }
        public string? VOEN { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? NoticePeriod { get; set; } = 0;
        [JsonIgnore]
        public int CreatedBy { get; set; }
        public Airway? AirwayData { get; set; }
        public Hotel? HotelData { get; set; }
        public List<ServicesCost>? CostData { get; set; }
    }
}
