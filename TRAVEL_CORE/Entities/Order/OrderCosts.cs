using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TRAVEL_CORE.Entities.Order
{
    public class OrderCosts
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int VenderId { get; set; }
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
