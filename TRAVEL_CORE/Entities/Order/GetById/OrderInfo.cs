namespace TRAVEL_CORE.Entities.Order.GetById
{
    public class OrderInfo
    {
        public int Id { get; set; }
        public string? OrderNo { get; set; }
        public short OrderType { get; set; }
        public DateTime Orderdate { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public AirwayById? AirwayData { get; set; }
        public Hotel? HotelData { get; set; }
        public List<ServicesCost>? CostData { get; set; }
    }
}
