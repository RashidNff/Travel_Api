using System.Data;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Contract;
using TRAVEL_CORE.Entities.TemplateCost;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface IContractRepository
    {
        DataTable GetContractBrowseData(FilterParameter filterParameter);
        ResponseModel SaveContract(ContractData saveContract);
        string GetContractNo();
        ContractById GetContractById(int contractId);
        ResponseModel ChangeStatus(ChangeStatus model);
    }
}
