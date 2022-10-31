using Microsoft.AspNetCore.Routing;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using TMTM2_Web_Api.Entities;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Repositories.Abstract;

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
            int generatedOrderId = 0, generatedAirId = 0, generatedHotelId =0;
            List<SqlParameter> orderParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderNo", order.OrderNo),
                    new SqlParameter("Orderdate", order.Orderdate),
                    new SqlParameter("FullName", order.FullName),
                    new SqlParameter("Phone", order.Phone),
                    new SqlParameter("Email", order.Email)
                };
            

            if (order.Id != 0)
            {

            }
            else
            {
                generatedOrderId = connection.Execute(tableName: "OPR.Orders", operation: OperationType.Insert, parameters: orderParameters);

                if (order.AirwayData != null)
                {
                    order.AirwayData.OrderId = generatedOrderId;
                    generatedAirId = SaveAirwayData(order.AirwayData);
                }
                if (order.HotelData != null)
                {
                    order.HotelData.OrderId = generatedOrderId;
                    generatedHotelId = SaveHotelData(order.HotelData);
                }

            }


            return 1;
        }


        private int SaveAirwayData(Airway airwayModel)
        {
            int id = 0, generatedPersonId =0;
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

                if (airwayModel.PersonDetails != null)
                {
                    generatedPersonId = SavePersonDetails(airwayModel.PersonDetails, id);
                }
            }
                
            return id;
        }


        private int SaveHotelData(Hotel model)
        {
            int id = 0;
            List<SqlParameter> hotelParameters = new List<SqlParameter>
            {
                    new SqlParameter("OrderId", model.OrderId),
                    new SqlParameter("HotelName", model.HotelName),
                    new SqlParameter("EnrtyDate", model.EnrtyDate),
                    new SqlParameter("ExitDate", model.ExitDate),
                    new SqlParameter("GuestCount", model.GuestCount),
                    new SqlParameter("RoomClassId", model.RoomClassId),
                    new SqlParameter("Bron", model.Bron),
                    new SqlParameter("BronExpiryDate", model.BronExpiryDate)
            };

            if (model.id == 0)
                id = connection.Execute(tableName: "OPR.Hotels", operation: OperationType.Insert, parameters: hotelParameters);
            else
                id = connection.Execute(tableName: "OPR.Hotels", operation: OperationType.Update, fieldName: "Id", ID: model.Id, parameters: hotelParameters);

            return id;
        }
        private int SavePersonDetails(List<PersonDetails>? personDetails, int operationId)
        {
            int id = 0;
            foreach (var personDetail in personDetails)
            {
                List<SqlParameter> personParameters = new List<SqlParameter>
                {
                    new SqlParameter("OperationId", operationId),
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

                if (personDetail.Id != 0)
                {
                    id = connection.Execute(tableName: "CRD.PersonDetails", operation: OperationType.Update, fieldName: "Id", ID: personDetail.Id, parameters: personParameters);
                }
                else
                {
                    id = connection.Execute(tableName: "CRD.PersonDetails", operation: OperationType.Insert, parameters: personParameters);

                    if (personDetail.AdditionalServices != null)
                        SaveAdditionalServices(personDetail.AdditionalServices, id);
                    if (personDetail.SpecialServices != null)
                        SaveSpecialServices(personDetail.SpecialServices, id);
                }
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
                    new SqlParameter("OperationType", additionalService.OperationType),
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
    }
}
