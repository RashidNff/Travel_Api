using System.Data;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface ICommonRepository
    {
        DataTable GetSpecode(string type);
        DataTable GetTemplateCosts();
        DataTable GetFirms();
        DataTable GetContractedFirms();
        DataTable GetFirmInfoById(int id);
        DataTable GetPersonInfoByDocNumber(int docType, string docNumber);
        DataTable GetPersonDocNumbers();
        DataTable GetAirport();
    }
}
