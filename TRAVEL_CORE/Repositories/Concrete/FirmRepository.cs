using System.Data;
using System.Data.SqlClient;
using System.Text;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Contract;
using TRAVEL_CORE.Entities.Firm;
using TRAVEL_CORE.Repositories.Abstract;

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class FirmRepository : IFirmRepository
    {
        Connection connection = new Connection();
        public DataTable GetFirmBrowseData(FilterParameter filterParameter)
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
                query = $@"Select F.Id, CompanyName, VOEN, Name + ' ' + Surname Fullname, Phone, Email,
                            CASE
	                            when F.Status = 3 then S.Value1
	                            else Cast(F.Status as nvarchar(20))
                            END Status,S.ColorCode  from CRD.Firms F
                            Left Join  OBJ.SpeCodes S ON S.RefId = F.Status and S.Type = 'OrderStatus' and S.Status = 1
                            WHERE F.CreatedDate between @FromDate and @ToDate {stringFilter}
                            Order by F.Id DESC";
            else
                query = $@"Select F.Id, CompanyName, VOEN, Name + ' ' + Surname Fullname, Phone, Email,
                            CASE
	                            when F.Status = 3 then S.Value1
	                            else Cast(F.Status as nvarchar(20))
                            END Status,S.ColorCode  from CRD.Firms F
                            Left Join  OBJ.SpeCodes S ON S.RefId = F.Status and S.Type = 'OrderStatus' and S.Status = 1
                            WHERE F.CreatedDate between @FromDate and @ToDate and F.Status = @OrderStatus {stringFilter}
                            Order by F.Id DESC";

            var data = connection.GetData(commandText: query, parameters: parameters);
            return data;
        }

        public int SaveFirm(FirmData saveFirm)
        {
            int generatedId = 0;
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                    new SqlParameter("Code", saveFirm.Code),
                    new SqlParameter("CompanyName", saveFirm.CompanyName),
                    new SqlParameter("VOEN", saveFirm.VOEN),
                    new SqlParameter("Name", saveFirm.Name),
                    new SqlParameter("Surname", saveFirm.Surname),
                    new SqlParameter("Phone", saveFirm.Phone),
                    new SqlParameter("Email", saveFirm.Email),
                    new SqlParameter("CreatedBy", saveFirm.CreatedBy)
            };

            if (saveFirm.Id != 0)
                generatedId = connection.Execute(tableName: "CRD.Firms", operation: OperationType.Update, fieldName: "Id", ID: saveFirm.Id, parameters: parameters);
            else
                generatedId = connection.Execute(tableName: "CRD.Firms", operation: OperationType.Insert, parameters: parameters);

            return generatedId;
        }

        public string GetFirmCode()
        {
            StringBuilder formatString = new();
            int number = 0;
            var reader = connection.RunQuery(commandText: "CRD.SP_GetLastFirmCode", commandType: CommandType.StoredProcedure);

            if (reader.Read())
            {
                if (!string.IsNullOrEmpty(reader["Code"].ToString()))
                    number = Convert.ToInt32(reader["Code"]);
                else
                    return "00001";
            }

            for (int i = 0; i <= 4; i++)
            {
                formatString.Append("0");
            }

            string formattedNumber = (number + 1).ToString(formatString.ToString());

            return $"{formattedNumber}";
        }

        public FirmData GetFirmById(int firmId)
        {
            FirmData firm = new();

            List<SqlParameter> Parameters = new List<SqlParameter>();
            Parameters.Add(new SqlParameter("Id", firmId));
            var reader = connection.RunQuery(commandText: "CRD.SP_GetFirmById", parameters: Parameters, commandType: CommandType.StoredProcedure);
            if (reader.Read())
            {
                firm.Id = Convert.ToInt32(reader["Id"]);
                firm.Code = reader["Code"].ToString(); ;
                firm.CompanyName = reader["CompanyName"].ToString();
                firm.VOEN = reader["VOEN"].ToString();
                firm.Name = reader["Name"].ToString();
                firm.Surname = reader["Surname"].ToString();
                firm.Phone = reader["Phone"].ToString();
                firm.Email = reader["Email"].ToString();
            }
            reader.Close();

            return firm;
        }
        public void ChangeOrderStatus(ChangeStatus model)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TableName", "CRD.Contract"));
            parameters.Add(new SqlParameter("Id", model.Id));
            parameters.Add(new SqlParameter("Status", model.Status));
            connection.RunQuery(commandText: "SP_CHANGESTATUS", parameters: parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
