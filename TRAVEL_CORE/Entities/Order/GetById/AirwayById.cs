namespace TRAVEL_CORE.Entities.Order.GetById
{
    public class AirwayById
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? FromPointId { get; set; }
        public string? FromPointName { get; set; }
        public int? ToPointId { get; set; }
        public string? ToPointName { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int FlightClassId { get; set; }
        public int PassengersCount { get; set; }
        public Boolean Bron { get; set; }
        public DateTime? BronExpiryDate { get; set; }
        public int? NoticePeriod { get; set; } = 0;
        public List<PersonCategoryCount>? CategoryCount { get; set; }

        public List<PersonDetailsById>? PersonDetails { get; set; }

    }
}
