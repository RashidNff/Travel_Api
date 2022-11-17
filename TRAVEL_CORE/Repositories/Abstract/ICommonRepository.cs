using System.Data;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface ICommonRepository
    {
        DataTable GetSpecode(string type);
        DataTable GetTemplateCosts();
        DataTable GetFirms();
        DataTable GetFirmInfoById(int id);
        DataTable GetPersonInfoByDocNumber(string docNumber);
        DataTable GetPersonDocNumbers();
    }
}
