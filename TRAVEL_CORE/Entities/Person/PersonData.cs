using System.Text.Json.Serialization;

namespace TRAVEL_CORE.Entities.Person
{
    public class PersonData
    {
        public int Id { get; set; }
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
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
}
