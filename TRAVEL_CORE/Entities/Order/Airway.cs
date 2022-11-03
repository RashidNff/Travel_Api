namespace TRAVEL_CORE.Entities.Order
{
    public class Airway
    {
        public int Id { get; set; }
        public string? FromPoint { get; set; }
        public string? ToPoint { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int FlightClassId { get; set; }
        public int PassengersCount { get; set; }
        public byte Bron { get; set; }
        public DateTime BronExpiryDate { get; set; }
        public List<PersonDetails>? PersonDetails { get; set; }
        public List<int>? DeletedPersonDetailIds { get; set; }
    }
}
