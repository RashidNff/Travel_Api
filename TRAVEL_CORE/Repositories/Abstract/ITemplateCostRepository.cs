using System.Data;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Entities.TemplateCost;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface ITemplateCostRepository
    {
        DataTable GetTemplateCostBrowseData();
        DataTable GetExpences(int templateCostId);
        TemplateCost GetTemplateCostById(int templateCostId);
        int SaveTemplateCost(TemplateCost templateCosts);
        void ChangeTemplateCostStatus(ChangeStatus model);
    }
}
