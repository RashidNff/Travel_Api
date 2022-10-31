namespace TRAVEL_CORE.Entities.Order
{
    public class Hotel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string? HotelName { get; set; }
        public DateTime EnrtyDate { get; set; }
        public DateTime ExitDate { get; set; }
        public int GuestCount { get; set; }
        public int RoomClassId { get; set; }
        public byte Bron { get; set; }
        public DateTime BronExpiryDate { get; set; }
        public List<PersonDetails>? PersonDetails { get; set; }
    }
}
