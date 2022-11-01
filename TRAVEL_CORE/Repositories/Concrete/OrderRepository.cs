﻿using Microsoft.AspNetCore.Routing;
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

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class OrderRepository : IOrderRepository
    {
        Connection connection = new Connection();

        public DataTable GetAirBrowseData(FilterParameter filterParameter)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("FromDate", filterParameter.FromDate));
            parameters.Add(new SqlParameter("ToDate", filterParameter.ToDate));
            var data = connection.GetData(commandText: "SP_GetAllAirBrowseData", parameters: parameters, commandType: CommandType.StoredProcedure);
            return data;
        }

        public int SaveOrder(SaveOrder order)
        {
            int generatedOrderId = 0;
            List<SqlParameter> orderParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderNo", order.OrderNo),
                    new SqlParameter("OrderType", order.OrderType),
                    new SqlParameter("Orderdate", order.Orderdate),
                    new SqlParameter("FullName", order.FullName),
                    new SqlParameter("Phone", order.Phone),
                    new SqlParameter("Email", order.Email)
                };

            if (order.Id != 0)
                generatedOrderId = connection.Execute(tableName: "OPR.Orders", operation: OperationType.Update, fieldName: "Id", ID: order.Id, parameters: orderParameters);
            else
                generatedOrderId = connection.Execute(tableName: "OPR.Orders", operation: OperationType.Insert, parameters: orderParameters);

            if (order.AirwayData != null)
            {
                order.AirwayData.OrderId = generatedOrderId;
                SaveAirwayData(order.AirwayData);
            }
            if (order.HotelData != null)
            {
                order.HotelData.OrderId = generatedOrderId;
                SaveHotelData(order.HotelData);
            }            
            
            if (order.CostData != null)
            {
                SaveCostData(order.CostData, generatedOrderId);
            }


            return generatedOrderId;
        }


        private void SaveAirwayData(Airway airwayModel)
        {
            int id = 0, generatedPersonId = 0;
            List<SqlParameter> airwayParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderId", airwayModel.OrderId),
                    new SqlParameter("FromPoint", airwayModel.FromPoint),
                    new SqlParameter("ToPoint", airwayModel.ToPoint),
                    new SqlParameter("DepartureDate", airwayModel.DepartureDate),
                    new SqlParameter("ReturnDate", airwayModel.ReturnDate),
                    new SqlParameter("FlightClassId", airwayModel.FlightClassId),
                    new SqlParameter("PassengersCount", airwayModel.PassengersCount),
                    new SqlParameter("Bron", airwayModel.Bron),
                    new SqlParameter("BronExpiryDate", airwayModel.BronExpiryDate)
                };

            if (airwayModel.Id != 0)
            {
                id = connection.Execute(tableName: "OPR.Airways", operation: OperationType.Update, fieldName: "Id", ID: airwayModel.Id, parameters: airwayParameters);
            }
            else
            {
                id = connection.Execute(tableName: "OPR.Airways", operation: OperationType.Insert, parameters: airwayParameters);
            }

            if (airwayModel.DeletedPersonDetailIds != null)
                DeletePersonAndServices(airwayModel.DeletedPersonDetailIds);

            if (airwayModel.PersonDetails != null)
                generatedPersonId = SavePersonDetails(airwayModel.PersonDetails, id, (int)OrderOperationType.Airway);
        }

        private void SaveHotelData(Hotel hotelModel)
        {
            int id = 0, generatedPersonId = 0;
            List<SqlParameter> hotelParameters = new List<SqlParameter>
            {
                    new SqlParameter("OrderId", hotelModel.OrderId),
                    new SqlParameter("HotelName", hotelModel.HotelName),
                    new SqlParameter("EntryDate", hotelModel.EnrtyDate),
                    new SqlParameter("ExitDate", hotelModel.ExitDate),
                    new SqlParameter("GuestCount", hotelModel.GuestCount),
                    new SqlParameter("RoomClassId", hotelModel.RoomClassId),
                    new SqlParameter("Bron", hotelModel.Bron),
                    new SqlParameter("BronExpiryDate", hotelModel.BronExpiryDate)
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
                    new SqlParameter("PersonAgeCategory", personDetail.PersonAgeCategory),
                    new SqlParameter("Name", personDetail.Name),
                    new SqlParameter("Surname", personDetail.Surname),
                    new SqlParameter("Gender", personDetail.Gender),
                    new SqlParameter("BirthDate", personDetail.BirthDate),
                    new SqlParameter("DocType", personDetail.DocType),
                    new SqlParameter("DocNumber", personDetail.DocNumber),
                    new SqlParameter("DocIssueCountry", personDetail.DocIssueCountry),
                    new SqlParameter("DocExpireDate", personDetail.DocExpireDate),
                    new SqlParameter("DocScan", personDetail.DocScan)
                };

                if (!string.IsNullOrEmpty(personDetail.DocName))
                {
                    FileOperation fileOperation = new FileOperation();
                    UploadedFile uploaded = fileOperation.MoveFile(personDetail.DocName, "PersonDetail");
                    personParameters.Add(new SqlParameter("DocScan", uploaded.FilePath));
                }

                if (personDetail.Id != 0)
                {
                    id = connection.Execute(tableName: "CRD.PersonDetails", operation: OperationType.Update, fieldName: "Id", ID: personDetail.Id, parameters: personParameters);
                }
                else
                {
                    id = connection.Execute(tableName: "CRD.PersonDetails", operation: OperationType.Insert, parameters: personParameters);
                }

                

                if (personDetail.DeletedAdditionalServiceIds != null)
                    DeleteAdditionalService(personDetail.DeletedAdditionalServiceIds);                
                if (personDetail.DeletedSpecialServiceIds != null)
                    DeleteSpecialService(personDetail.DeletedSpecialServiceIds);

                if (personDetail.AdditionalServices != null)
                    SaveAdditionalServices(personDetail.AdditionalServices, id);
                if (personDetail.SpecialServices != null)
                    SaveSpecialServices(personDetail.SpecialServices, id);

            }
            
            return id;
        }


        private void SaveSpecialServices(List<SpecialServices>? specialServices, int personId)
        {
            foreach (var specialService in specialServices)
            {
                List<SqlParameter> specialParameters = new List<SqlParameter>
                {
                    new SqlParameter("PersonId", personId),
                    new SqlParameter("ServiciesId", specialService.ServiciesId),
                    new SqlParameter("Type", specialService.Type)
                };

                if (specialService.Id != 0)
                    connection.Execute(tableName: "CRD.SpecialServices", operation: OperationType.Update, fieldName: "Id", ID: specialService.Id, parameters: specialParameters);
                else
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

                if (additionalService.Id != 0)
                    connection.Execute(tableName: "CRD.AdditionalServices", operation: OperationType.Update, fieldName: "Id", ID: additionalService.Id, parameters: additionalParameters);
                else
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
                    new SqlParameter("OrderId", cost.OrderId),
                    new SqlParameter("Vender", cost.Vender),
                    new SqlParameter("VenderService", cost.VenderService),
                    new SqlParameter("Qty", cost.Qty),
                    new SqlParameter("VenderUnitPrice", cost.VenderUnitPrice),
                    new SqlParameter("VenderAmount", cost.VenderAmount),
                    new SqlParameter("SaleUnitPrice", cost.SaleUnitPrice),
                    new SqlParameter("SaleAmount", cost.SaleAmount),
                    new SqlParameter("VAT", cost.VAT),
                    new SqlParameter("Currency", cost.Currency),
                    new SqlParameter("CurrencyAmount", cost.CurrencyAmount)
                };

                connection.Execute(tableName: "OPR.ServicesCost", operation: OperationType.Insert, parameters: costParameters);
            }
            
        }

        public OrderInfo GetOrderById(int ordId)
        {
            OrderInfo orderInfo = new();
            AirwayById airwayInfo = new();
            HotelById hotelInfo = new();
            
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("OrderId", ordId));
            var reader = connection.RunQuery(commandText: "CRD.SP_GetOrderInfo", parameters: parameters, commandType: CommandType.StoredProcedure);
            if (reader.Read())
            {
                orderInfo.Id = Convert.ToInt32(reader["Id"]);
                orderInfo.OrderNo = reader["OrderNo"].ToString();
                orderInfo.OrderType = Convert.ToInt16(reader["OrderType"].ToString());
                orderInfo.FullName = reader["Fullname"].ToString();
                orderInfo.Phone = reader["Phone"].ToString();
                orderInfo.Email = reader["Email"].ToString();
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
                airwayInfo.Bron = Convert.ToByte(readerAir["PassengersCount"].ToString());
                airwayInfo.BronExpiryDate = Convert.ToDateTime(readerAir["BronExpiryDate"].ToString());

                List<PersonDetailsById> personList = new();
                GetPersonDataById(personList, airwayInfo.Id);

                airwayInfo.PersonDetails = personList;
            }
            readerAir.Close();

            ////////////HotelData
            var readerHotel = connection.RunQuery(commandText: "CRD.SP_GetHotelByOrderId", parameters: parameters, commandType: CommandType.StoredProcedure);

            if (readerHotel.Read())
            {
                hotelInfo.Id = Convert.ToInt32(readerHotel["Id"]);
                hotelInfo.OrderId = Convert.ToInt32(readerHotel["OrderId"].ToString());
                hotelInfo.HotelName = readerHotel["HotelName"].ToString();
                hotelInfo.EnrtyDate = Convert.ToDateTime(readerHotel["EnrtyDate"].ToString());
                hotelInfo.ExitDate = Convert.ToDateTime(readerHotel["ExitDate"].ToString());
                hotelInfo.GuestCount = Convert.ToInt32(readerHotel["GuestCount"].ToString());
                hotelInfo.RoomClassId = Convert.ToInt32(readerHotel["RoomClassId"].ToString());
                hotelInfo.Bron = Convert.ToBoolean(readerHotel["Bron"].ToString());
                hotelInfo.BronExpiryDate = Convert.ToDateTime(readerHotel["BronExpiryDate"].ToString());

                List<PersonDetailsById> personList = new();
                GetPersonDataById(personList, hotelInfo.Id);

                hotelInfo.PersonDetails = personList;
            }
            readerHotel.Close();

            orderInfo.AirwayData = airwayInfo;
            orderInfo.HotelData = hotelInfo;

            ////////////CostData
            var costLines = connection.GetData(commandText: "CRD.SP_GetCostByOrderId", parameters: parameters, commandType: CommandType.StoredProcedure);
            orderInfo.CostData = JsonConvert.DeserializeObject<List<ServicesCost>>(JsonConvert.SerializeObject(costLines));

            return orderInfo;
        }

        private void GetPersonDataById(List<PersonDetailsById> personList, int Id)
        {
            List<SqlParameter> personParameters = new List<SqlParameter>();
            personParameters.Add(new SqlParameter("OperationId", Id));
            personParameters.Add(new SqlParameter("OperationType", (int)OrderOperationType.Airway));


            var readerPerson = connection.RunQuery(commandText: "CRD.SP_GetPersonByOperationId", parameters: personParameters, commandType: CommandType.StoredProcedure);

            while (readerPerson.Read())
            {
                PersonDetailsById personDetailsById = new();
                personDetailsById.Id = Convert.ToInt32(readerPerson["Id"]);
                personDetailsById.PersonAgeCategory = Convert.ToInt32(readerPerson["PersonAgeCategory"].ToString());
                personDetailsById.Name = readerPerson["Name"].ToString();
                personDetailsById.Surname = readerPerson["Surname"].ToString();
                personDetailsById.Gender = Convert.ToInt16(readerPerson["Gender"].ToString());
                personDetailsById.BirthDate = Convert.ToDateTime(readerPerson["BirthDate"].ToString());
                personDetailsById.DocType = Convert.ToInt32(readerPerson["DocType"].ToString());
                personDetailsById.Surname = readerPerson["DocNumber"].ToString();
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
    }
}
