using System.Text.Json.Serialization;

namespace TRAVEL_CORE.Entities.Contract
{
    public class ContractData
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string? ContractNo { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
    }
}
