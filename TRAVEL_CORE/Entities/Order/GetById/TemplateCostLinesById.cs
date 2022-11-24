namespace TRAVEL_CORE.Entities.Order.GetById
{
    public class TemplateCostLinesById
    {
        public int? VenderId { get; set; }
        public int VenderService { get; set; }
        public int Qty { get; set; }
        public float VenderUnitPrice { get; set; }
        public decimal VenderAmount { get; set; }
        public decimal SaleUnitPrice { get; set; }
        public decimal SaleAmount { get; set; }
        public int Currency { get; set; }
        public float CurrencyRate { get; set; }
        public decimal CurrencyAmount { get; set; }
        public bool Status { get; set; }

    }
}
