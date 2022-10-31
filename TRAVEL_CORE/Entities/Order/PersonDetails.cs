namespace TRAVEL_CORE.Entities.Order
{
    public class PersonDetails
    {
        public int Id { get; set; }
        public int PersonAgeCategory { get; set; }
        public short Operationİd { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public short Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public int DocType { get; set; }
        public string? DocNumber { get; set; }
        public int DocIssueCountry { get; set; }
        public DateTime DocExpireDate { get; set; }
        public int DocScan { get; set; }
        public List<AdditionalServices>? AdditionalServices { get; set; }
        public List<SpecialServices>? SpecialServices { get; set; }
    }
}
