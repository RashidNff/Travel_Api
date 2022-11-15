using System.Text.Json.Serialization;

namespace TRAVEL_CORE.Entities.Firm
{
    public class FirmData
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? CompanyName { get; set; }
        public string? VOEN { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
}
