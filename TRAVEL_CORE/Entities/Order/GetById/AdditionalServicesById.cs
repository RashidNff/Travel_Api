namespace TRAVEL_CORE.Entities.Order.GetById
{
    public class AdditionalServicesById
    {
        public int Id { get; set; }
        public int AdditionalId { get; set; }
        public string? DepartureService { get; set; }
        public string? ReturnService { get; set; }
    }
}
