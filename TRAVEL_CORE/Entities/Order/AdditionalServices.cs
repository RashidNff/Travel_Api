namespace TRAVEL_CORE.Entities.Order
{
    public class AdditionalServices
    {
        public int Id { get; set; }
        public int OperationType { get; set; }
        public int PersonId { get; set; }
        public int AdditionalType { get; set; }
        public string? DepartureService { get; set; }
        public string? ReturnService { get; set; }
    }
}
