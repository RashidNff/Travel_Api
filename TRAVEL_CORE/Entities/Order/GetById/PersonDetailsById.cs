namespace TRAVEL_CORE.Entities.Order.GetById
{
    public class PersonDetailsById
    {
        public int Id { get; set; }
        public int Category { get; set; }
        public string? PersonAgeName { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public short Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int DocType { get; set; }
        public string? DocNumber { get; set; }
        public string? DocIssueCountry { get; set; }
        public DateTime DocExpireDate { get; set; }
        public string? DocScan { get; set; }
        public List<AdditionalServices>? AdditionalServices { get; set; }
        public List<SpecialServices>? SpecialServices  { get; set; }
    }
}
