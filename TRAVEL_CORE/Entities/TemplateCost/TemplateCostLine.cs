namespace TRAVEL_CORE.Entities.TemplateCost
{
    public class TemplateCostLine
    {
        public string? VenderId { get; set; }
        public int VenderService { get; set; }
        public int Qty { get; set; }
        public float VenderUnitPrice { get; set; }
        public decimal VenderAmount { get; set; }
        public decimal SaleUnitPrice { get; set; }
        public decimal SaleAmount { get; set; }
        public int Currency { get; set; }
        public string? CurrencyName { get; set; }
        public float CurrencyRate { get; set; }
        public decimal CurrencyAmount { get; set; }
    }
}
