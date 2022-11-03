using System.Data;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Entities.TemplateCost;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface ITemplateCostRepository
    {
        DataTable GetTemplateCostBrowseData(TempCostFilterParametr filterParameter);
        TemplateCost GetTemplateCostById(int templateCostId);
        int SaveTemplateCost(TemplateCost templateCosts);
        void ChangeTemplateCostStatus(int templateCostId, int status);
    }
}
