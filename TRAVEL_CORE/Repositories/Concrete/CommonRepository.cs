using System.Data.SqlClient;
using System.Data;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Repositories.Abstract;

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class CommonRepository: ICommonRepository
    {
        Connection connection = new Connection();
        public DataTable GetSpecode(string type)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("type", type));
            var data = connection.GetData(commandText: "SP_GETSPECODE", parameters: parameters, commandType: CommandType.StoredProcedure);
            return data;
        }
    }
}
