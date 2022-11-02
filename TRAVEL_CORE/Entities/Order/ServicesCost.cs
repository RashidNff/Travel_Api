﻿namespace TRAVEL_CORE.Entities.Order
{
    public class ServicesCost
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string? Vender { get; set; }
        public int VenderService { get; set; }
        public int Qty { get; set; }
        public float VenderUnitPrice { get; set; }
        public decimal VenderAmount { get; set; }
        public decimal SaleUnitPrice { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal VAT { get; set; }
        public int Currency { get; set; }
        public decimal CurrencyAmount { get; set; }
    }
}