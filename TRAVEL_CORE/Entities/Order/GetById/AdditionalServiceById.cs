namespace TRAVEL_CORE.Entities.Order.GetById
{
    public class AdditionalServiceById
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int AdditionalId { get; set; }
        public string? DepartureService { get; set; }
        public string? ReturnService { get; set; }
    }
}
