using Microsoft.AspNetCore.Routing;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Tools;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Repositories.Abstract;
using TRAVEL_CORE.Enums;
using System.Linq;
using TRAVEL_CORE.Entities.Order.GetById;
using Newtonsoft.Json;
using TRAVEL_CORE.Entities;
using System.Xml;
using System.Text;
using TRAVEL_CORE.Entities.TemplateCost;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class OrderRepository : IOrderRepository
    {
        Connection connection = new Connection();

        public DataTable GetOrderBrowseData(FilterParameter filterParameter)
        {
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


            var query = $@"Select OrderNo,Ord.ID,
                            --AirWay
                            CompanyName, FullName, Phone, FromPoint, ToPoint, Convert(varchar, DepartureDate, 105) DepartureDate, Convert(varchar, ReturnDate, 105) ReturnDate, PassengersCount,
                            Case 
	                            when Air.Bron = 0  then null
	                            else Convert(varchar, Air.BronExpiryDate, 105)
                            End AirwayBronExpiryDate,
                            --Hotel
                            HotelName, Convert(varchar, EntryDate, 105) EntryDate, Convert(varchar, ExitDate, 105) ExitDate, GuestCount, PCOUNT RoomCount,
                            Case 
	                            when H.Bron = 0 then null
	                            else Convert(varchar, H.BronExpiryDate, 105)
                            End HotelBronExpiryDate,
                            Convert(varchar, Orderdate, 105) Orderdate,
                            Sc.SaleAmount,SC.AznAmount,
                            Ord.Status
                            from OPR.Orders Ord
                            Left Join  OPR.Airways Air ON Air.OrderId = Ord.Id and Air.Status = 1
                            Left Join  OPR.Hotels H ON H.OrderId = Ord.Id and H.Status = 1
                            Left Join  (SELECT OperationId,COUNT(*) PCOUNT FROM CRD.PersonDetails WHERE OperationType=2 GROUP BY OperationId) P ON p.OperationId = H.Id 
                            Left Join  (SELECT OrderId,SUM(SaleAmount) SaleAmount,--CurrencyRate rate,
                            SUM(CurrencyAmount) AznAmount 
                            FROM OPR.ServicesCost  GROUP BY OrderId --,CurrencyRate
                            ) SC ON SC.OrderId = Ord.Id 
                            WHERE Orderdate between @FromDate and @ToDate {stringFilter}
                            Order by Ord.ID desc";

            var data = connection.GetData(commandText: query, parameters: parameters);
            return data;
        }

        public int SaveOrder(SaveOrder order)
        {
            int generatedOrderId = 0;
            List<SqlParameter> orderParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderNo", order.OrderNo),
                    new SqlParameter("OrderType", order.OrderType),
                    new SqlParameter("OrderDate", order.OrderDate),
                    new SqlParameter("CompanyName", order.CompanyName),
                    new SqlParameter("VOEN", order.VOEN),
                    new SqlParameter("FullName", order.FullName),
                    new SqlParameter("Phone", order.Phone),
                    new SqlParameter("Email", order.Email),
                    new SqlParameter("CreatedBy", order.CreatedBy)
                };

            if (order.Id != 0)
                generatedOrderId = connection.Execute(tableName: "OPR.Orders", operation: OperationType.Update, fieldName: "Id", ID: order.Id, parameters: orderParameters);
            else
                generatedOrderId = connection.Execute(tableName: "OPR.Orders", operation: OperationType.Insert, parameters: orderParameters);


            if (order.AirwayData != null)
                SaveAirwayData(order.AirwayData, generatedOrderId);
            else
                DeleteAirwayData(generatedOrderId);

            if (order.HotelData != null)
                SaveHotelData(order.HotelData, generatedOrderId);
            else
                DeleteHotelData(generatedOrderId);
            
            if (order.CostData.Count != 0)
                SaveCostData(order.CostData, generatedOrderId);


            return generatedOrderId;
        }



        private void SaveAirwayData(Airway airwayModel, int orderId)
        {
            int id = 0, generatedPersonId = 0;
            List<SqlParameter> airwayParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderId", orderId),
                    new SqlParameter("FromPoint", airwayModel.FromPoint),
                    new SqlParameter("ToPoint", airwayModel.ToPoint),
                    new SqlParameter("DepartureDate", airwayModel.DepartureDate),
                    new SqlParameter("ReturnDate", airwayModel.ReturnDate),
                    new SqlParameter("FlightClassId", airwayModel.FlightClassId),
                    new SqlParameter("PassengersCount", airwayModel.PassengersCount),
                    new SqlParameter("Bron", airwayModel.Bron),
                    new SqlParameter("BronExpiryDate", airwayModel.BronExpiryDate?.ToString("yyyy-MM-dd") ?? String.Empty)
                };

            if (airwayModel.Id != 0)
                id = connection.Execute(tableName: "OPR.Airways", operation: OperationType.Update, fieldName: "Id", ID: airwayModel.Id, parameters: airwayParameters);
            else
                id = connection.Execute(tableName: "OPR.Airways", operation: OperationType.Insert, parameters: airwayParameters);


            if (airwayModel.DeletedPersonDetailIds != null)
                DeletePersonAndServices(airwayModel.DeletedPersonDetailIds);

            if (airwayModel.PersonDetails != null)
                generatedPersonId = SavePersonDetails(airwayModel.PersonDetails, id, (int)OrderOperationType.Airway);
        }

        private void SaveHotelData(Hotel hotelModel, int orderId)
        {
            int id = 0, generatedPersonId = 0;
            List<SqlParameter> hotelParameters = new List<SqlParameter>
            {
                    new SqlParameter("OrderId", orderId),
                    new SqlParameter("HotelName", hotelModel.HotelName),
                    new SqlParameter("EntryDate", hotelModel.EnrtyDate),
                    new SqlParameter("ExitDate", hotelModel.ExitDate),
                    new SqlParameter("GuestCount", hotelModel.GuestCount),
                    new SqlParameter("RoomClassId", hotelModel.RoomClassId),
                    new SqlParameter("Bron", hotelModel.Bron),
                    new SqlParameter("BronExpiryDate", hotelModel.BronExpiryDate?.ToString("yyyy-MM-dd") ?? String.Empty)
            };

            if (hotelModel.Id != 0)
            {
                id = connection.Execute(tableName: "OPR.Hotels", operation: OperationType.Update, fieldName: "Id", ID: hotelModel.Id, parameters: hotelParameters);
            }
            else
            {
                id = connection.Execute(tableName: "OPR.Hotels", operation: OperationType.Insert, parameters: hotelParameters);
            }

            if (hotelModel.DeletedPersonDetailIds != null)
                DeletePersonAndServices(hotelModel.DeletedPersonDetailIds);

            if (hotelModel.PersonDetails != null)
                generatedPersonId = SavePersonDetails(hotelModel.PersonDetails, id, (int)OrderOperationType.Hotel);
        }
        private int SavePersonDetails(List<PersonDetails>? personDetails, int operationId, int operationType)
        {
            int id = 0;
            foreach (var personDetail in personDetails)
            {
                List<SqlParameter> personParameters = new List<SqlParameter>
                {
                    new SqlParameter("OperationId", operationId),
                    new SqlParameter("OperationType", operationType),
                    new SqlParameter("Category", personDetail.Category),
                    new SqlParameter("Name", personDetail.Name),
                    new SqlParameter("Surname", personDetail.Surname),
                    new SqlParameter("Gender", personDetail.Gender),
                    new SqlParameter("BirthDate", personDetail.BirthDate),
                    new SqlParameter("DocType", personDetail.DocType),
                    new SqlParameter("DocNumber", personDetail.DocNumber),
                    new SqlParameter("DocIssueCountry", personDetail.DocIssueCountry),
                    new SqlParameter("DocExpireDate", personDetail.DocExpireDate),
                };

                if (!string.IsNullOrEmpty(personDetail.DocName))
                {
                    FileOperation fileOperation = new FileOperation();
                    UploadedFile uploaded = fileOperation.MoveFile(personDetail.DocName, "PersonDetail");
                    personParameters.Add(new SqlParameter("DocScan", uploaded.FilePath));
                }

                if (personDetail.Id != 0)
                    id = connection.Execute(tableName: "CRD.PersonDetails", operation: OperationType.Update, fieldName: "Id", ID: personDetail.Id, parameters: personParameters);
                else
                    id = connection.Execute(tableName: "CRD.PersonDetails", operation: OperationType.Insert, parameters: personParameters);
                

                DeleteServices(id);                

                if (personDetail.AdditionalServices != null)
                    SaveAdditionalServices(personDetail.AdditionalServices, id);
                if (personDetail.SpecialServices != null)
                    SaveSpecialServices(personDetail.SpecialServices, id);
            }
            
            return id;
        }

        private void DeleteServices(int personId)
        {
            connection.Execute(tableName: "CRD.AdditionalServices", operation: OperationType.Delete, fieldName: "PersonId", ID: personId);
            connection.Execute(tableName: "CRD.SpecialServices", operation: OperationType.Delete, fieldName: "PersonId", ID: personId);
        }

        private void DeleteAirwayData(int generatedOrderId)
        {
            connection.Execute(tableName: "CRD.Airways", operation: OperationType.Delete, fieldName: "OrderId", ID: generatedOrderId);
        }
        private void DeleteHotelData(int generatedOrderId)
        {
            connection.Execute(tableName: "CRD.Hotels", operation: OperationType.Delete, fieldName: "OrderId", ID: generatedOrderId);
        }
        private void SaveSpecialServices(List<int>? specialServices, int personId)
        {
            foreach (var specialService in specialServices)
            {
                List<SqlParameter> specialParameters = new List<SqlParameter>
                {
                    new SqlParameter("PersonId", personId),
                    new SqlParameter("ServicesId", specialService),
                };

                connection.Execute(tableName: "CRD.SpecialServices", operation: OperationType.Insert, parameters: specialParameters);
            }
        }

        private void SaveAdditionalServices(List<AdditionalServices>? additionalServices, int personId)
        {
            foreach (var additionalService in additionalServices)
            {
                List<SqlParameter> additionalParameters = new List<SqlParameter>
                {
                    new SqlParameter("PersonId", personId),
                    new SqlParameter("AdditionalId", additionalService.AdditionalId),
                    new SqlParameter("DepartureService", additionalService.DepartureService),
                    new SqlParameter("ReturnService", additionalService.ReturnService)
                };

                connection.Execute(tableName: "CRD.AdditionalServices", operation: OperationType.Insert, parameters: additionalParameters);
            }
        }

        private void DeleteAdditionalService(List<int> additionalServiceIds)
        {
            foreach (var additionalServiceId in additionalServiceIds)
            {
                connection.Execute(tableName: "CRD.AdditionalServices", operation: OperationType.Delete, fieldName: "Id", ID: additionalServiceId);
            }
        }

        private void DeleteSpecialService(List<int> deletedSpecialServiceIds)
        {
            foreach (var specialServiceId in deletedSpecialServiceIds)
            {
                connection.Execute(tableName: "CRD.SpecialServices", operation: OperationType.Delete, fieldName: "Id", ID: specialServiceId);
            }
        }

        private void DeletePersonAndServices(List<int> deletedPersonDetailIds)
        {
            foreach (var personDetailId in deletedPersonDetailIds)
            {
                connection.Execute(tableName: "CRD.PersonDetails", operation: OperationType.Delete, fieldName: "Id", ID: personDetailId);
                connection.Execute(tableName: "CRD.AdditionalServices", operation: OperationType.Delete, fieldName: "PersonId", ID: personDetailId);
                connection.Execute(tableName: "CRD.SpecialServices", operation: OperationType.Delete, fieldName: "PersonId", ID: personDetailId);
            }
        }

        private void SaveCostData(List<ServicesCost> costData, int orderId)
        {
            connection.Execute(tableName: "OPR.ServicesCost", operation: OperationType.Delete, fieldName: "OrderId", ID: orderId);

            foreach (var cost in costData)
            {
                List<SqlParameter> costParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderId", orderId),
                    new SqlParameter("Vender", cost.Vender),
                    new SqlParameter("VenderService", cost.VenderService),
                    new SqlParameter("Qty", cost.Qty),
                    new SqlParameter("VenderUnitPrice", cost.VenderUnitPrice),
                    new SqlParameter("VenderAmount", cost.VenderAmount),
                    new SqlParameter("SaleUnitPrice", cost.SaleUnitPrice),
                    new SqlParameter("SaleAmount", cost.SaleAmount),
                    new SqlParameter("Currency", cost.Currency),
                    new SqlParameter("CurrencyRate", cost.CurrencyRate),
                    new SqlParameter("CurrencyAmount", cost.CurrencyAmount)
                };

                connection.Execute(tableName: "OPR.ServicesCost", operation: OperationType.Insert, parameters: costParameters);
            }
            
        }

        public OrderInfo GetOrderById(int ordId)
        {
            OrderInfo orderInfo = new();
            AirwayById? airwayInfo = new();
            HotelById? hotelInfo = new();
            
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("OrderId", ordId));
            var reader = connection.RunQuery(commandText: "CRD.SP_GetOrderInfoById", parameters: parameters, commandType: CommandType.StoredProcedure);
            if (reader.Read())
            {
                orderInfo.Id = Convert.ToInt32(reader["Id"]);
                orderInfo.OrderNo = reader["OrderNo"].ToString();
                orderInfo.Orderdate = Convert.ToDateTime(reader["Orderdate"].ToString());
                orderInfo.OrderType = Convert.ToInt16(reader["OrderType"].ToString());
                orderInfo.FullName = reader["Fullname"].ToString();
                orderInfo.Phone = reader["Phone"].ToString();
                orderInfo.Email = reader["Email"].ToString();
                var fdfdf = reader["CompanyName"].ToString();
                orderInfo.CompanyName = reader["CompanyName"].ToString();
                orderInfo.VOEN = reader["VOEN"].ToString();
            }
            reader.Close();

            ////////////AirwayData
            var readerAir = connection.RunQuery(commandText: "CRD.SP_GetAirwayByOrderId", parameters: parameters, commandType: CommandType.StoredProcedure);
            if (readerAir.Read())
            {
                airwayInfo.Id = Convert.ToInt32(readerAir["Id"]);
                airwayInfo.OrderId = Convert.ToInt32(readerAir["OrderId"].ToString());
                airwayInfo.FromPoint = readerAir["FromPoint"].ToString();
                airwayInfo.ToPoint = readerAir["ToPoint"].ToString();
                airwayInfo.DepartureDate = Convert.ToDateTime(readerAir["DepartureDate"].ToString());
                airwayInfo.ReturnDate = Convert.ToDateTime(readerAir["ReturnDate"].ToString());
                airwayInfo.FlightClassId = Convert.ToInt32(readerAir["FlightClassId"].ToString());
                airwayInfo.PassengersCount = Convert.ToInt32(readerAir["PassengersCount"].ToString());
                airwayInfo.Bron = Convert.ToBoolean(readerAir["Bron"].ToString());

                if (readerAir["BronExpiryDate"].ToString() != "")
                    airwayInfo.BronExpiryDate = Convert.ToDateTime(readerAir["BronExpiryDate"]);
                
                List<PersonCategoryCount> personAgeCountsList = new();
                personAgeCountsList = GetPersonAgeCount(personAgeCountsList, airwayInfo.Id, OrderOperationType.Airway);

                List<PersonDetailsById> personList = new();
                GetPersonDataById(personList, airwayInfo.Id, OrderOperationType.Airway);

                airwayInfo.CategoryCount = personAgeCountsList;
                airwayInfo.PersonDetails = personList;
            }
            else
                airwayInfo = null;

            readerAir.Close();

            ////////////HotelData
            var readerHotel = connection.RunQuery(commandText: "CRD.SP_GetHotelByOrderId", parameters: parameters, commandType: CommandType.StoredProcedure);

            if (readerHotel.Read())
            {
                hotelInfo.Id = Convert.ToInt32(readerHotel["Id"]);
                hotelInfo.OrderId = Convert.ToInt32(readerHotel["OrderId"].ToString());
                hotelInfo.HotelName = readerHotel["HotelName"].ToString();
                hotelInfo.EnrtyDate = Convert.ToDateTime(readerHotel["EntryDate"].ToString());
                hotelInfo.ExitDate = Convert.ToDateTime(readerHotel["ExitDate"].ToString());
                hotelInfo.GuestCount = Convert.ToInt32(readerHotel["GuestCount"].ToString());
                hotelInfo.RoomClassId = Convert.ToInt32(readerHotel["RoomClassId"].ToString());
                hotelInfo.Bron = Convert.ToBoolean(readerHotel["Bron"].ToString());

                if (readerHotel["BronExpiryDate"].ToString() != "")
                    hotelInfo.BronExpiryDate = Convert.ToDateTime(readerHotel["BronExpiryDate"]);

                List<PersonCategoryCount> roomCountsList = new();
                roomCountsList = GetPersonAgeCount(roomCountsList, hotelInfo.Id, OrderOperationType.Hotel);

                List<PersonDetailsById> personList = new();
                GetPersonDataById(personList, hotelInfo.Id, OrderOperationType.Hotel);

                hotelInfo.CategoryCount = roomCountsList;
                hotelInfo.PersonDetails = personList;
            }
            else
                hotelInfo = null;

            readerHotel.Close();

            orderInfo.AirwayData = airwayInfo;
            orderInfo.HotelData = hotelInfo;

            ////////////CostData
            var costLines = connection.GetData(commandText: "CRD.SP_GetCostByOrderId", parameters: parameters, commandType: CommandType.StoredProcedure);
            orderInfo.CostData = JsonConvert.DeserializeObject<List<ServicesCost>>(JsonConvert.SerializeObject(costLines));

            return orderInfo;
        }

        private List<PersonCategoryCount> GetPersonAgeCount(List<PersonCategoryCount>? personAgeCountsList, int operationId, OrderOperationType operationType)
        {
            List<SqlParameter> countParametrs = new List<SqlParameter>();
            countParametrs.Add(new SqlParameter("OperationType", operationType));
            countParametrs.Add(new SqlParameter("OperationId", operationId));

            var ageCountlines = connection.GetData(commandText: "CRD.SP_GetPersonAgeCategoryCount", parameters: countParametrs, commandType: CommandType.StoredProcedure);
            return JsonConvert.DeserializeObject<List<PersonCategoryCount>>(JsonConvert.SerializeObject(ageCountlines));
        }

        private void GetPersonDataById(List<PersonDetailsById> personList, int Id, OrderOperationType orderOperationType)
        {
            List<SqlParameter> personParameters = new List<SqlParameter>();
            personParameters.Add(new SqlParameter("OperationId", Id));
            personParameters.Add(new SqlParameter("OperationType", (int)orderOperationType));


            var readerPerson = connection.RunQuery(commandText: "CRD.SP_GetPersonByOperationId", parameters: personParameters, commandType: CommandType.StoredProcedure);

            while (readerPerson.Read())
            {
                PersonDetailsById personDetailsById = new();
                personDetailsById.Id = Convert.ToInt32(readerPerson["Id"]);
                personDetailsById.Category = Convert.ToInt32(readerPerson["Category"].ToString());
                personDetailsById.PersonAgeName = readerPerson["PersonAgeName"].ToString();
                personDetailsById.Name = readerPerson["Name"].ToString();
                personDetailsById.Surname = readerPerson["Surname"].ToString();
                personDetailsById.Gender = Convert.ToInt16(readerPerson["Gender"].ToString());
                personDetailsById.BirthDate = Convert.ToDateTime(readerPerson["BirthDate"].ToString());
                personDetailsById.DocType = Convert.ToInt32(readerPerson["DocType"].ToString());
                personDetailsById.DocNumber = readerPerson["DocNumber"].ToString();
                personDetailsById.DocIssueCountry = readerPerson["DocIssueCountry"].ToString();
                personDetailsById.DocExpireDate = Convert.ToDateTime(readerPerson["DocExpireDate"].ToString());
                personDetailsById.DocScan = readerPerson["DocScan"].ToString();



                List<SqlParameter> additionalAndSpecial = new List<SqlParameter>();
                additionalAndSpecial.Add(new SqlParameter("PersonId", personDetailsById.Id));

                var additionallines = connection.GetData(commandText: "CRD.SP_GetAdditionalServicesByPersonId", parameters: additionalAndSpecial, commandType: CommandType.StoredProcedure);
                personDetailsById.AdditionalServices = JsonConvert.DeserializeObject<List<AdditionalServices>>(JsonConvert.SerializeObject(additionallines));

                var speciallines = connection.GetData(commandText: "CRD.SP_GetSpecialServicesByPersonId", parameters: additionalAndSpecial, commandType: CommandType.StoredProcedure);
                personDetailsById.SpecialServices = JsonConvert.DeserializeObject<List<SpecialServices>>(JsonConvert.SerializeObject(speciallines));

                personList.Add(personDetailsById);
            }
        }

        public string GetOrderNo()
        {
            StringBuilder formatString = new();
            int number = 0;
            string prefix = "ARTW";
            var reader = connection.RunQuery(commandText: "CRD.SP_GetLastOrderNo", commandType: CommandType.StoredProcedure);

            if (reader.Read())
            {
                if (!string.IsNullOrEmpty(reader["OrderNo"].ToString()))
                    number = Convert.ToInt32(reader["OrderNo"].ToString()?.Split('-')[1]);
                else
                    return "ARTW-00001";
            }

            for (int i = 0; i <= 4; i++)
            {
                formatString.Append("0");
            }

            string formattedNumber = (number + 1).ToString(formatString.ToString());

            return $"{prefix}-{formattedNumber}";
        }

        public List<TemplateCostLinesById> GetTemplateCostData(int templateCostId)
        {
            List<TemplateCostLinesById>? templateCosts = new();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Id", templateCostId));

            var costlines = connection.GetData(commandText: "CRD.SP_GetTemplateCostLinesById", parameters: parameters, commandType: CommandType.StoredProcedure);
            templateCosts = JsonConvert.DeserializeObject<List<TemplateCostLinesById>>(JsonConvert.SerializeObject(costlines));

            return templateCosts;
        }

        public void ChangeOrderStatus(ChangeStatus model)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TableName", "OPR.Orders"));
            parameters.Add(new SqlParameter("Id", model.Id));
            parameters.Add(new SqlParameter("Status", model.Status));
            connection.RunQuery(commandText: "SP_CHANGESTATUS", parameters: parameters, commandType: CommandType.StoredProcedure);
        }

        public void SendMail()
        {
            OrderMail orderInfo = new();

            var reader = connection.RunQuery(commandText: "OPR.SP_GetOrderInfo", commandType: CommandType.StoredProcedure);
            while (reader.Read())
            {
                orderInfo.OrderNo = reader["OrderNo"].ToString();
                orderInfo.Orderdate = reader["Orderdate"].ToString();
                orderInfo.FullName = reader["Fullname"].ToString();
                orderInfo.Email = reader["Email"].ToString();
                orderInfo.OperationType = reader["OperationType"].ToString();
                orderInfo.BronExpiryDate = reader["BronExpiryDate"].ToString();

                string message = $"<div style=\"font-size:16px\">Salam, {orderInfo.FullName}! <br/><br/>{orderInfo.Orderdate} tarixində verilən <b>{orderInfo.OrderNo} </b> nömrəli sifarişin bron müddətinin bitməsinə 1 gün qalıb.<br/><br/><br/> <b style=\"margin-right:40px\">Əməliyyatın tipi:</b> {orderInfo.OperationType}<br/><b style=\"margin-right:30px\">Bron bitmə tarixi:</b> {orderInfo.BronExpiryDate}<br/> <b>Ətraflı məlumat üçün əlaqə:</b> 055 555 55 55<br/></div>";
                CommonTools.SendEmail("shahin.mustafayev@uniser.az", "Məlumatlandırma", message);
            }
            reader.Close();
        }
    }
}
