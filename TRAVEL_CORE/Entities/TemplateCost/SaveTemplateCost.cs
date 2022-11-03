namespace TRAVEL_CORE.Entities.TemplateCost
{
    public class TemplateCost
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CreateBy { get; set; }
        public List<TemplateCostLine>? templateCostLines { get; set; }
    }
}
