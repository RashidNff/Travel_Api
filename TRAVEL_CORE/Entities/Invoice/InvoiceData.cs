namespace TRAVEL_CORE.Entities.Invoice
{
    public class InvoiceData
    {
        public string? InvoiceNo { get; set; }
        public string? OrderNo { get; set; }
        public string? CreatedDate { get; set; }
        public string? Name { get; set; }
        public string? Currency { get; set; }
        public List<InvoiceCost>? InvoiceCosts { get; set; }
    }
}
