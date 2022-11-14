using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities.Invoice;
using TRAVEL_CORE.Entities.TemplateCost;
using TRAVEL_CORE.Repositories.Abstract;
using System.Text;
using TRAVEL_CORE.Entities.Login;
using TRAVEL_CORE.Tools;

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class InvoiceRepository : IInvoiceRepository
    {
        Connection connection = new Connection();
        public InvoiceData GetInvoiceById(int ordId)
        {
            InvoiceData invoiceData = new();

            List<SqlParameter> Parameters = new List<SqlParameter>();
            Parameters.Add(new SqlParameter("OrderId", ordId));
            var reader = connection.RunQuery(commandText: "CRD.SP_CheckInvoiceData", parameters: Parameters, commandType: CommandType.StoredProcedure);

            if (!reader.Read())
            {
                invoiceData.InvoiceNo = GetInvoiceNo();
                Parameters.Add(new SqlParameter("InvoiceNo", invoiceData.InvoiceNo));
                connection.Execute(tableName: "CRD.Invoice", operation: OperationType.Insert, parameters: Parameters);
            }
            reader.Close();

            Parameters.Clear();
            Parameters.Add(new SqlParameter("OrderId", ordId));

            var reader2 = connection.RunQuery(commandText: "CRD.SP_GetInvoiceData", parameters: Parameters, commandType: CommandType.StoredProcedure);

            if (reader2.Read())
            {
                invoiceData.InvoiceNo = reader2["InvoiceNo"].ToString();
                invoiceData.OrderNo = reader2["OrderNo"].ToString();
                invoiceData.CreatedDate = reader2["CreatedDate"].ToString();
                invoiceData.Name = reader2["Name"].ToString();
                invoiceData.Currency = reader2["Currency"].ToString();
            }

            var costlines = connection.GetData(commandText: "CRD.SP_GetInvoiceCosts", parameters: Parameters, commandType: CommandType.StoredProcedure);
            invoiceData.InvoiceCosts = JsonConvert.DeserializeObject<List<InvoiceCost>>(JsonConvert.SerializeObject(costlines));

            return invoiceData;
        }

        private string GetInvoiceNo()
        {
            StringBuilder formatString = new();
            int number = 0;
            string prefix = $"INV-AZE {DateTime.Now.Year}/";
            var reader = connection.RunQuery(commandText: "CRD.SP_GetLastInvoiceNo", commandType: CommandType.StoredProcedure);

            if (reader.Read())
            {
                if (!string.IsNullOrEmpty(reader["InvoiceNo"].ToString()))
                    number = Convert.ToInt32(reader["InvoiceNo"].ToString()?.Split('/')[1]);
                else
                    return $"INV-AZE {DateTime.Now.Year}/001";
            }

            for (int i = 0; i <= 2; i++)
            {
                formatString.Append("0");
            }

            string formattedNumber = (number + 1).ToString(formatString.ToString());

            return $"{prefix}{formattedNumber}";
        }
    }
}
