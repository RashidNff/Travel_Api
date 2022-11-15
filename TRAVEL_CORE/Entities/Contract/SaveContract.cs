namespace TRAVEL_CORE.Entities.Contract
{
    public class SaveContract
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string? ContractNo { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
