using System.Data;
using TMTM2_Web_Api.Entities;
using TRAVEL_CORE.Entities.Order;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface IOrderRepository
    {
        DataTable GetAirBrowseData(FilterParameter filterParameter);
        int SaveOrder(SaveOrder order);

    }
}
