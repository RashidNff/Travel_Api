namespace TRAVEL_CORE.Entities.Order
{
    public class Hotel
    {
        public int Id { get; set; }
        public string? HotelName { get; set; }
        public DateTime EnrtyDate { get; set; }
        public DateTime ExitDate { get; set; }
        public int GuestCount { get; set; }
        public int RoomClassId { get; set; }
        public Boolean Bron { get; set; }
        public DateTime? BronExpiryDate { get; set; }
        public int? NoticePeriod { get; set; }
        public List<PersonDetails>? PersonDetails { get; set; }
        public List<int>? DeletedPersonDetailIds { get; set; }

    }
}
