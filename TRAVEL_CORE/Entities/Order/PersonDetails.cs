namespace TRAVEL_CORE.Entities.Order
{
    public class PersonDetails
    {
        public int OrderPersonId { get; set; }
        public int PersonId { get; set; }
        public int Category { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public short Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int DocType { get; set; }
        public string? DocNumber { get; set; }
        public string? DocIssueCountry { get; set; }
        public DateTime DocExpireDate { get; set; }
        public string? DocScan { get; set; }
        public string? DocName { get; set; }
        public List<AdditionalServices>? AdditionalServices { get; set; }
        public List<int>? SpecialServices { get; set; }

    }
}
