using System.Data;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Entities.Order.GetById;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface IOrderRepository
    {
        DataTable GetAirBrowseData(FilterParameter filterParameter);
        OrderInfo GetOrderById(int ordId);
        int SaveOrder(SaveOrder order);
        string GetOrderNo();
    }
}
