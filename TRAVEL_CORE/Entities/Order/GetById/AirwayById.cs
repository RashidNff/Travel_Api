namespace TRAVEL_CORE.Entities.Order.GetById
{
    public class AirwayById
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string? FromPoint { get; set; }
        public string? ToPoint { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int FlightClassId { get; set; }
        public int PassengersCount { get; set; }
        public Boolean Bron { get; set; }
        public DateTime BronExpiryDate { get; set; }
        public List<PersonDetailsById>? PersonDetails { get; set; }

    }
}
