using System.Data;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Entities.Order.GetById;
using TRAVEL_CORE.Entities.TemplateCost;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface IOrderRepository
    {
        DataTable GetOrderBrowseData(FilterParameter filterParameter);
        OrderInfo GetOrderById(int ordId);
        ResponseModel SaveOrder(SaveOrder order);
        string GetOrderNo();
        List<TemplateCostLinesById> GetTemplateCostData(int templateCostId);
        ResponseModel ChangeOrderStatus(ChangeStatus model);
        List<OrderCosts> GetOrderCostsById(int ordId);
        ResponseModel SaveOrderCosts(List<OrderCosts> costs);


    }
}
