using System.Data;
using TRAVEL_CORE.Entities.Firm;
using TRAVEL_CORE.Entities;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface IFirmRepository
    {
        DataTable GetFirmBrowseData(FilterParameter filterParameter);
        int SaveFirm(FirmData saveFirm);
        string GetFirmCode();
        FirmData GetFirmById(int contractId);
        void ChangeStatus(ChangeStatus model, bool contractCheck);
    }
}
