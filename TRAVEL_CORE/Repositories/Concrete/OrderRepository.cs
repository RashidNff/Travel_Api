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
using System.Globalization;
using System.Security.Cryptography;
using System.Reflection.Metadata;
using Microsoft.Extensions.Hosting;

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class OrderRepository : IOrderRepository
    {
        Connection connection = new Connection();

        public DataTable GetOrderBrowseData(FilterParameter filterParameter)
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
                query = $@"Select OrderNo,Ord.ID, F.CompanyName, FullName, Ord.Phone,
                            --AirWay
                             ALF.IATA +' - '+ ALF.Region FromPoint,ALT.IATA +' - '+ ALT.Region ToPoint,  Convert(varchar, DepartureDate, 105) DepartureDate, Convert(varchar, ReturnDate, 105) ReturnDate, PassengersCount,
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
                            Sc.SaleAmount,SC.AznAmount,Ord.Status, S.ColorCode
                            from OPR.Orders Ord
                            Left Join  OPR.Airways Air ON Air.OrderId = Ord.Id and Air.Status = 1
                            Left Join  OPR.Hotels H ON H.OrderId = Ord.Id and H.Status = 1
                            Left Join  OBJ.SpeCodes S ON S.RefId = Ord.Status and S.Type = 'OrderStatus' and S.Status = 1
							Left Join  CRD.Firms F ON F.Id = Ord.CompanyId
							Left Join  OBJ.AirportList ALF ON ALF.Id = Air.FromPoint
							Left Join  OBJ.AirportList ALT ON ALT.Id = Air.ToPoint
                            Left Join  (SELECT OperationId,COUNT(*) PCOUNT FROM CRD.OrderPerson WHERE OperationType=2 GROUP BY OperationId) P ON p.OperationId = H.Id 
                            Left Join  (SELECT OrderId,SUM(SaleAmount) SaleAmount,--CurrencyRate rate,
                            SUM(CurrencyAmount) AznAmount 
                            FROM OPR.ServicesCost where Status = 1  GROUP BY OrderId --,CurrencyRate
                            ) SC ON SC.OrderId = Ord.Id 
                            WHERE Orderdate between @FromDate and @ToDate {stringFilter}
                            Order by Ord.ID desc";
            else
                query = $@"Select OrderNo,Ord.ID, F.CompanyName, FullName, Ord.Phone,
                            --AirWay
                            ALF.IATA +' - '+ ALF.Region FromPoint,ALT.IATA +' - '+ ALT.Region ToPoint, Convert(varchar, DepartureDate, 105) DepartureDate, Convert(varchar, ReturnDate, 105) ReturnDate, PassengersCount,
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
                            Sc.SaleAmount,SC.AznAmount,Ord.Status, S.ColorCode
                            from OPR.Orders Ord
                            Left Join  OPR.Airways Air ON Air.OrderId = Ord.Id and Air.Status = 1
                            Left Join  OPR.Hotels H ON H.OrderId = Ord.Id and H.Status = 1
                            Left Join  OBJ.SpeCodes S ON S.RefId = Ord.Status and S.Type = 'OrderStatus' and S.Status = 1
							Left Join  CRD.Firms F ON F.Id = Ord.CompanyId
							Left Join  OBJ.AirportList ALF ON ALF.Id = Air.FromPoint
							Left Join  OBJ.AirportList ALT ON ALT.Id = Air.ToPoint
                            Left Join  (SELECT OperationId,COUNT(*) PCOUNT FROM CRD.OrderPerson WHERE OperationType=2 GROUP BY OperationId) P ON p.OperationId = H.Id 
                            Left Join  (SELECT OrderId,SUM(SaleAmount) SaleAmount,--CurrencyRate rate,
                            SUM(CurrencyAmount) AznAmount 
                            FROM OPR.ServicesCost  where Status = 1  GROUP BY OrderId --,CurrencyRate
                            ) SC ON SC.OrderId = Ord.Id 
                            WHERE Orderdate between @FromDate and @ToDate and Ord.Status = @OrderStatus {stringFilter}
                            Order by Ord.ID desc";


            var data = connection.GetData(commandText: query, parameters: parameters);
            return data;
        }

        public ResponseModel SaveOrder(SaveOrder order)
        {
            int generatedOrderId = 0;
            List<SqlParameter> orderParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderNo", order.OrderNo),
                    new SqlParameter("OrderType", order.OrderType),
                    new SqlParameter("OrderDate", order.OrderDate),
                    new SqlParameter("VOEN", order.VOEN),
                    new SqlParameter("FullName", order.FullName),
                    new SqlParameter("Phone", order.Phone),
                    new SqlParameter("Email", order.Email),
                    new SqlParameter("CreatedBy", order.CreatedBy)
                };

            if (order.CompanyId != 0)
                orderParameters.Add(new SqlParameter("CompanyId", order.CompanyId));

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

            DeleteCostData(generatedOrderId);
            if (order.CostData.Count != 0)
                SaveCostData(order.CostData, generatedOrderId);


            return new ResponseModel { Message = CommonTools.GetMessage((int)MessageCodes.Save), Status = true, Data = generatedOrderId };
        }

        private void DeleteCostData(int generatedOrderId)
        {
            string query = $@"Delete OPR.ServicesCost where OrderId = {generatedOrderId} and Status = 1";
            var data = connection.GetData(commandText: query);
            //connection.Execute(tableName: "OPR.ServicesCost", operation: OperationType.Delete, fieldName: "OrderId", ID: generatedOrderId);
        }

        private void SaveAirwayData(Airway airwayModel, int orderId)
        {
            int id = 0, generatedPersonId = 0;
            List<SqlParameter> airwayParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderId", orderId),
                    new SqlParameter("FromPoint", airwayModel.FromPointId),
                    new SqlParameter("ToPoint", airwayModel.ToPointId),
                    new SqlParameter("DepartureDate", airwayModel.DepartureDate),
                    new SqlParameter("ReturnDate", airwayModel.ReturnDate?.ToString("yyyy-MM-dd HH:mm") ?? String.Empty),
                    new SqlParameter("FlightClassId", airwayModel.FlightClassId),
                    new SqlParameter("PassengersCount", airwayModel.PassengersCount),
                    new SqlParameter("Bron", airwayModel.Bron),
                    new SqlParameter("BronExpiryDate", airwayModel.BronExpiryDate?.ToString("yyyy-MM-dd HH:mm") ?? String.Empty),
                    new SqlParameter("NoticePeriod", airwayModel.NoticePeriod)
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
                    new SqlParameter("BronExpiryDate", hotelModel.BronExpiryDate?.ToString("yyyy-MM-dd HH:mm") ?? String.Empty),
                    new SqlParameter("NoticePeriod", hotelModel.NoticePeriod)
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
                //List<SqlParameter> personParameters = new List<SqlParameter>
                //{
                //    new SqlParameter("OperationId", operationId),
                //    new SqlParameter("OperationType", operationType),
                //    new SqlParameter("Category", personDetail.Category),
                //    new SqlParameter("Name", personDetail.Name),
                //    new SqlParameter("Surname", personDetail.Surname),
                //    new SqlParameter("Gender", personDetail.Gender),
                //    new SqlParameter("BirthDate", personDetail.BirthDate),
                //    new SqlParameter("DocType", personDetail.DocType),
                //    new SqlParameter("DocNumber", personDetail.DocNumber),
                //    new SqlParameter("DocIssueCountry", personDetail.DocIssueCountry),
                //    new SqlParameter("DocExpireDate", personDetail.DocExpireDate)
                //};

                //if (!string.IsNullOrEmpty(personDetail.DocName))
                //{
                //    FileOperation fileOperation = new FileOperation();
                //    UploadedFile uploaded = fileOperation.MoveFile(personDetail.DocName, "PersonDetail");
                //    personParameters.Add(new SqlParameter("DocScan", uploaded.FilePath));
                //}
                //else
                //    personParameters.Add(new SqlParameter("DocScan", ""));

                List<SqlParameter> personParameters = new List<SqlParameter>
                {
                    new SqlParameter("PersonId", personDetail.PersonId),
                    new SqlParameter("OperationId", operationId),
                    new SqlParameter("OperationType", operationType),
                    new SqlParameter("Category", personDetail.Category)
                };

                if (personDetail.OrderPersonId != 0)
                    id = connection.Execute(tableName: "CRD.OrderPerson", operation: OperationType.Update, fieldName: "Id", ID: personDetail.OrderPersonId, parameters: personParameters);
                else
                    id = connection.Execute(tableName: "CRD.OrderPerson", operation: OperationType.Insert, parameters: personParameters);
                

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
            foreach (var cost in costData)
            {
                List<SqlParameter> costParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderId", orderId),
                    new SqlParameter("Vender", cost.VenderId),
                    new SqlParameter("VenderService", cost.VenderService),
                    new SqlParameter("Qty", cost.Qty),
                    new SqlParameter("VenderUnitPrice", cost.VenderUnitPrice),
                    new SqlParameter("VenderAmount", cost.VenderAmount),
                    new SqlParameter("SaleUnitPrice", cost.SaleUnitPrice),
                    new SqlParameter("SaleAmount", cost.SaleAmount),
                    new SqlParameter("Currency", cost.Currency),
                    new SqlParameter("CurrencyRate", Math.Round(cost.CurrencyRate,4)),
                    new SqlParameter("CurrencyAmount", cost.CurrencyAmount),
                    new SqlParameter("Status", cost.Status)
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
                orderInfo.Orderdate = Convert.ToDateTime(reader["Orderdate"].ToString(), CultureInfo.CreateSpecificCulture("en-GB"));
                orderInfo.OrderType = Convert.ToInt16(reader["OrderType"].ToString());
                orderInfo.FullName = reader["Fullname"].ToString();
                orderInfo.Phone = reader["Phone"].ToString();
                orderInfo.Email = reader["Email"].ToString();
                orderInfo.CompanyName = reader["CompanyName"].ToString();
                orderInfo.CompanyId = Convert.ToInt32(reader["CompanyId"].ToString());
                orderInfo.VOEN = reader["VOEN"].ToString();
            }
            reader.Close();

            ////////////AirwayData
            var readerAir = connection.RunQuery(commandText: "CRD.SP_GetAirwayByOrderId", parameters: parameters, commandType: CommandType.StoredProcedure);
            if (readerAir.Read())
            {
                airwayInfo.Id = Convert.ToInt32(readerAir["Id"]);
                airwayInfo.OrderId = Convert.ToInt32(readerAir["OrderId"].ToString());
                airwayInfo.FromPointId = Convert.ToInt32(readerAir["FromPointId"]);
                airwayInfo.FromPointName = readerAir["FromPointName"].ToString();
                airwayInfo.ToPointId = Convert.ToInt32(readerAir["ToPointId"]);
                airwayInfo.ToPointName = readerAir["ToPointName"].ToString();
                airwayInfo.DepartureDate = Convert.ToDateTime(readerAir["DepartureDate"].ToString(), CultureInfo.CreateSpecificCulture("en-GB"));
                airwayInfo.FlightClassId = Convert.ToInt32(readerAir["FlightClassId"].ToString());
                airwayInfo.PassengersCount = Convert.ToInt32(readerAir["PassengersCount"].ToString());
                airwayInfo.Bron = Convert.ToBoolean(readerAir["Bron"].ToString());

                if (readerAir["ReturnDate"].ToString() != "")
                {
                    airwayInfo.ReturnDate = Convert.ToDateTime(readerAir["ReturnDate"].ToString(), CultureInfo.CreateSpecificCulture("en-GB"));
                }
                if (readerAir["BronExpiryDate"].ToString() != "")
                {
                    airwayInfo.BronExpiryDate = Convert.ToDateTime(readerAir["BronExpiryDate"], CultureInfo.CreateSpecificCulture("en-GB"));
                    airwayInfo.NoticePeriod = Convert.ToInt32(readerAir["NoticePeriod"]);
                }
                
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
                hotelInfo.EnrtyDate = Convert.ToDateTime(readerHotel["EntryDate"].ToString(), CultureInfo.CreateSpecificCulture("en-GB"));
                hotelInfo.ExitDate = Convert.ToDateTime(readerHotel["ExitDate"].ToString(), CultureInfo.CreateSpecificCulture("en-GB"));
                hotelInfo.GuestCount = Convert.ToInt32(readerHotel["GuestCount"].ToString());
                hotelInfo.RoomClassId = Convert.ToInt32(readerHotel["RoomClassId"].ToString());
                hotelInfo.Bron = Convert.ToBoolean(readerHotel["Bron"].ToString());

                if (readerHotel["BronExpiryDate"].ToString() != "")
                {
                    hotelInfo.BronExpiryDate = Convert.ToDateTime(readerHotel["BronExpiryDate"], CultureInfo.CreateSpecificCulture("en-GB"));
                    hotelInfo.NoticePeriod = Convert.ToInt32(readerHotel["NoticePeriod"]);
                }

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
                personDetailsById.OrderPersonId = Convert.ToInt32(readerPerson["OrderPersonId"]);
                personDetailsById.PersonId = Convert.ToInt32(readerPerson["PersonId"]);
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
                additionalAndSpecial.Add(new SqlParameter("PersonId", personDetailsById.OrderPersonId));

                var additionallines = connection.GetData(commandText: "CRD.SP_GetAdditionalServicesByPersonId", parameters: additionalAndSpecial, commandType: CommandType.StoredProcedure);
                personDetailsById.AdditionalServices = JsonConvert.DeserializeObject<List<AdditionalServices>>(JsonConvert.SerializeObject(additionallines));

                var speciallinesReader = connection.RunQuery(commandText: "CRD.SP_GetSpecialServicesByPersonId", parameters: additionalAndSpecial, commandType: CommandType.StoredProcedure);
                List<int> specialList = new();
                while(speciallinesReader.Read())
                {
                    specialList.Add(Convert.ToInt32(speciallinesReader["ServicesId"]));
                }
                personDetailsById.SpecialServices = specialList;
                //personDetailsById.SpecialServices = JsonConvert.DeserializeObject<List<int>>(speciallines);

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

        public ResponseModel ChangeOrderStatus(ChangeStatus model)
        {
            int type = 0;
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TableName", "OPR.Orders"));
            parameters.Add(new SqlParameter("Id", model.Id));
            parameters.Add(new SqlParameter("Status", model.Status));
            connection.RunQuery(commandText: "SP_CHANGESTATUS", parameters: parameters, commandType: CommandType.StoredProcedure);

            if (model.Status == 1)
                type = (int)MessageCodes.Active;
            else
                type = (int)MessageCodes.Deactive;

            return new ResponseModel { Message = CommonTools.GetMessage(type), Status = true, Data = null };
        }

        public List<OrderCosts> GetOrderCostsById(int ordId)
        {
            List<OrderCosts>? orderCosts = new();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("OrderId", ordId));

            var costLines = connection.GetData(commandText: "CRD.SP_GetOrderCostsByOrderId", parameters: parameters, commandType: CommandType.StoredProcedure);
            orderCosts = JsonConvert.DeserializeObject<List<OrderCosts>>(JsonConvert.SerializeObject(costLines));

            return orderCosts;
        }

        public ResponseModel SaveOrderCosts(List<OrderCosts> costs, int orderId)
        {
            foreach (var cost in costs)
            {
                List<SqlParameter> Parameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderId", cost.OrderId),
                    new SqlParameter("Vender", cost.VenderId),
                    new SqlParameter("VenderService", cost.VenderService),
                    new SqlParameter("Qty", cost.Qty),
                    new SqlParameter("VenderUnitPrice", cost.VenderUnitPrice),
                    new SqlParameter("VenderAmount", cost.VenderAmount),
                    new SqlParameter("SaleUnitPrice", cost.SaleUnitPrice),
                    new SqlParameter("SaleAmount", cost.SaleAmount),
                    new SqlParameter("Currency", cost.Currency),
                    new SqlParameter("CurrencyRate", Math.Round(cost.CurrencyRate,4)),
                    new SqlParameter("CurrencyAmount", cost.CurrencyAmount),
                    new SqlParameter("Status", cost.Status)
                };

            if (cost.Id != 0)
                connection.Execute(tableName: "OPR.ServicesCost", operation: OperationType.Update, fieldName: "Id", ID: cost.Id, parameters: Parameters);
            else
                connection.Execute(tableName: "OPR.ServicesCost", operation: OperationType.Insert, parameters: Parameters);
            }

            List<SqlParameter> orderParameters = new List<SqlParameter>();
            orderParameters.Add(new SqlParameter("Status", 2));
            connection.Execute(tableName: "OPR.Orders", operation: OperationType.Update, fieldName: "Id", ID: orderId, parameters: orderParameters);


            return new ResponseModel { Message = CommonTools.GetMessage((int)MessageCodes.Save), Status = true, Data = null };
        }

        public List<int> GetBookingData()
        {
            List<int>? orderIds = new();

            var reader = connection.RunQuery(commandText: "CRD.SP_GetBookingData", commandType: CommandType.StoredProcedure);
            while (reader.Read())
                orderIds.Add(Convert.ToInt32(reader["OrderId"]));

            return orderIds;
        }

    }
}
