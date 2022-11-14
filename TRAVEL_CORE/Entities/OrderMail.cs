namespace TRAVEL_CORE.Entities
{
    public class OrderMail
    {
        public string OrderNo { get; set; }
        public string Orderdate { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string OperationType { get; set; }
        public string BronExpiryDate { get; set; }
        public int AirId { get; set; }
        public int HotelId { get; set; }
    }
}
