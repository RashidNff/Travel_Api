namespace TRAVEL_CORE.Entities.TemplateCost
{
    public class TemplateCostLine
    {
        public int? VenderId { get; set; }
        public string? VenderName { get; set; } = string.Empty;
        public int VenderService { get; set; }
        public string? VenderServiceName { get; set; } = string.Empty;
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
