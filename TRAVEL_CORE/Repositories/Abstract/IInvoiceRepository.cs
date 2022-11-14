using TRAVEL_CORE.Entities.Invoice;
using TRAVEL_CORE.Entities.Order.GetById;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface IInvoiceRepository
    {
        InvoiceData GetInvoiceById(int ordId);

    }
}
