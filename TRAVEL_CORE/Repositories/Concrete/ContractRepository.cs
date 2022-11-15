using System.Data;
using System.Data.SqlClient;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.TemplateCost;
using TRAVEL_CORE.Repositories.Abstract;

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class ContractRepository : IContractRepository
    {
        Connection connection = new Connection();

        public DataTable GetContractBrowseData(FilterParameter filterParameter)
        {
            string query = "";
            string stringFilter = "";
            if (filterParameter.Filters != null)
            {
                foreach (var filter in filterParameter.Filters)
                {
                    stringFilter += $"and {filter.Key} Like N'%{filter.Value}%'";
                }
            }

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("FromDate", filterParameter.FromDate));
            parameters.Add(new SqlParameter("ToDate", filterParameter.ToDate));
            parameters.Add(new SqlParameter("OrderStatus", filterParameter.OrderStatus));

            if (filterParameter.OrderStatus == 0)
                query = $@"Select C.Id, ContractNo, CL.CompanyName, Convert(varchar, C.BeginDate, 105) BeginDate, Convert(varchar, C.EndDate, 105) EndDate, 
                            CASE
	                            when C.Status = 3 then S.Value1
	                            else Cast(C.Status as nvarchar(20))
                            END
                            Status, S.ColorCode, Convert(varchar, C.CreatedDate, 105) CreatedDate  from CRD.Contract C
                            Left JOIN CRD.Client CL ON CL.Id = C.ClientId and Cl.Status = 1
                            Left Join  OBJ.SpeCodes S ON S.RefId = C.Status and S.Type = 'OrderStatus' and S.Status = 1
                            WHERE C.CreatedDate between @FromDate and @ToDate {stringFilter}
                            Order by C.Id DESC";
            else
                query = $@"Select C.Id, ContractNo, CL.CompanyName, Convert(varchar, C.BeginDate, 105) BeginDate, Convert(varchar, C.EndDate, 105) EndDate, 
                            CASE
	                            when C.Status = 3 then S.Value1
	                            else Cast(C.Status as nvarchar(20))
                            END
                            Status, S.ColorCode, Convert(varchar, C.CreatedDate, 105) CreatedDate  from CRD.Contract C
                            Left JOIN CRD.Client CL ON CL.Id = C.ClientId and Cl.Status = 1
                            Left Join  OBJ.SpeCodes S ON S.RefId = C.Status and S.Type = 'OrderStatus' and S.Status = 1
                            WHERE C.CreatedDate between @FromDate and @ToDate and C.Status = @OrderStatus {stringFilter}
                            Order by C.Id DESC";

            var data = connection.GetData(commandText: query, parameters: parameters);
            return data;
        }

        public int SaveTemplateCost(TemplateCost templateCosts)
        {
            throw new NotImplementedException();
        }
    }
}
