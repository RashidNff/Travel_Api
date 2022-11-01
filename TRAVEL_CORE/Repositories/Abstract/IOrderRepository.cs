using System.Data;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Order;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface IOrderRepository
    {
        DataTable GetAirBrowseData(FilterParameter filterParameter);
        OrderInfo GetOrderById(int ordId);
        int SaveOrder(SaveOrder order);

    }
}
