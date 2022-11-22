namespace TRAVEL_CORE.Entities.Order
{
    public class Airway
    {
        public int Id { get; set; }
        public int? FromPoint { get; set; }
        public int? ToPoint { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int FlightClassId { get; set; }
        public int PassengersCount { get; set; }
        public Boolean Bron { get; set; }
        public DateTime? BronExpiryDate { get; set; }
        public int? NoticePeriod { get; set; }
        public List<PersonDetails>? PersonDetails { get; set; }
        public List<int>? DeletedPersonDetailIds { get; set; }
    }
}
