using System.Data;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface ICommonRepository
    {
        DataTable GetSpecode(string type);
    }
}
