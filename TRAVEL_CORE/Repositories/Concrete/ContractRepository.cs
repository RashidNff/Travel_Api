using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Contract;
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
                query = $@"Select C.Id, ContractNo, F.CompanyName, Convert(varchar, C.BeginDate, 105) BeginDate, Convert(varchar, C.EndDate, 105) EndDate, C.Status, S.ColorCode, 
                            Convert(varchar, C.CreatedDate, 105) CreatedDate  from CRD.Contract C
                            Left JOIN CRD.Firms F ON F.Id = C.ClientId and F.Status = 1
                            Left Join  OBJ.SpeCodes S ON S.RefId = C.Status and S.Type = 'OrderStatus' and S.Status = 1
                            WHERE C.CreatedDate between @FromDate and @ToDate {stringFilter}
                            Order by C.Id DESC";
            else
                query = $@"Select C.Id, ContractNo, F.CompanyName, Convert(varchar, C.BeginDate, 105) BeginDate, Convert(varchar, C.EndDate, 105) EndDate, C.Status, S.ColorCode, 
                            Convert(varchar, C.CreatedDate, 105) CreatedDate  from CRD.Contract C
                            Left JOIN CRD.Firms F ON F.Id = C.ClientId and F.Status = 1
                            Left Join  OBJ.SpeCodes S ON S.RefId = C.Status and S.Type = 'OrderStatus' and S.Status = 1
                            WHERE C.CreatedDate between @FromDate and @ToDate and C.Status = @OrderStatus {stringFilter}
                            Order by C.Id DESC";

            var data = connection.GetData(commandText: query, parameters: parameters);
            return data;
        }

        public int SaveContract(ContractData saveContract)
        {
            int generatedId = 0;
            List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("ClientId", saveContract.ClientId),
                    new SqlParameter("ContractNo", saveContract.ContractNo),
                    new SqlParameter("BeginDate", saveContract.BeginDate),
                    new SqlParameter("EndDate", saveContract.EndDate),
                    new SqlParameter("CreatedBy", saveContract.CreatedBy)
                };

            if (saveContract.Id != 0)
                generatedId = connection.Execute(tableName: "CRD.Contract", operation: OperationType.Update, fieldName: "Id", ID: saveContract.Id, parameters: parameters);
            else
                generatedId = connection.Execute(tableName: "CRD.Contract", operation: OperationType.Insert, parameters: parameters);

            return generatedId;
        }

        public string GetContractNo()
        {
            StringBuilder formatString = new();
            int number = 0;
            string prefix = $"{DateTime.Now.ToString("yy")}/";
            var reader = connection.RunQuery(commandText: "CRD.SP_GetLastContractNo", commandType: CommandType.StoredProcedure);

            if (reader.Read())
            {
                if (!string.IsNullOrEmpty(reader["ContractNo"].ToString()))
                    number = Convert.ToInt32(reader["ContractNo"].ToString()?.Substring((int)reader["ContractNo"].ToString().IndexOf("/") + 1, 3));
                else
                    return $"{DateTime.Now.ToString("yy")}/001-MLT";
            }

            for (int i = 0; i <= 2; i++)
            {
                formatString.Append("0");
            }

            string formattedNumber = (number + 1).ToString(formatString.ToString());

            return $"{prefix}{formattedNumber}-MLT";
        }

        public ContractById GetContractById(int contractId)
        {
            ContractById contract = new();

            List<SqlParameter> Parameters = new List<SqlParameter>();
            Parameters.Add(new SqlParameter("Id", contractId));
            var reader = connection.RunQuery(commandText: "CRD.SP_GetContractById", parameters: Parameters, commandType: CommandType.StoredProcedure);
            if (reader.Read())
            {
                contract.Id = Convert.ToInt32(reader["Id"]);
                contract.ClientId = Convert.ToInt32(reader["ClientId"]);
                contract.CompanyName = reader["CompanyName"].ToString();
                contract.ContractNo = reader["ContractNo"].ToString();
                contract.BeginDate = Convert.ToDateTime(reader["BeginDate"].ToString(), CultureInfo.CreateSpecificCulture("en-GB"));
                contract.EndDate = Convert.ToDateTime(reader["EndDate"].ToString(), CultureInfo.CreateSpecificCulture("en-GB"));
            }
            reader.Close();

            return contract;
        }

        public void ChangeStatus(ChangeStatus model)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TableName", "CRD.Contract"));
            parameters.Add(new SqlParameter("Id", model.Id));
            parameters.Add(new SqlParameter("Status", model.Status));
            connection.RunQuery(commandText: "SP_CHANGESTATUS", parameters: parameters, commandType: CommandType.StoredProcedure);
        }

    }
}
