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

            

            if (order.Id == 0)
            {

            }
            else
            {
                generatedOrderId = connection.Execute(tableName: "OPR.Orders", operation: OperationType.Insert, parameters: orderParameters);

                if (order.AirwayData != null)
                {
                    generatedAirId = SaveAirwayData(order.AirwayData);
                }
                if (order.HotelData != null)
                {
                    generatedHotelId = SaveHotelData(order.HotelData);
                }

            }


            return 1;
        }


        private int SaveAirwayData(Airway model)
        {
            int id = 0;
            List<SqlParameter> airwayParameters = new List<SqlParameter>
                {
                    new SqlParameter("OrderId", model.OrderId),
                    new SqlParameter("FromPoint", model.FromPoint),
                    new SqlParameter("ToPoint", model.ToPoint),
                    new SqlParameter("DepartureDate", model.DepartureDate),
                    new SqlParameter("ReturnDate", model.ReturnDate),
                    new SqlParameter("FlightClassId", model.FlightClassId),
                    new SqlParameter("PassengersCount", model.PassengersCount),
                    new SqlParameter("Bron", model.Bron),
                    new SqlParameter("BronExpiryDate", model.BronExpiryDate)
                };

            if (model.OrderId == 0)
                id = connection.Execute(tableName: "OPR.Airways", operation: OperationType.Insert, parameters: airwayParameters);
            else
                id = connection.Execute(tableName: "OPR.Hotels", operation: OperationType.Update, fieldName: "Id", ID: model.Id, parameters: airwayParameters);

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

            if (model.OrderId == 0)
                id = connection.Execute(tableName: "OPR.Hotels", operation: OperationType.Insert, parameters: hotelParameters);
            else
                id = connection.Execute(tableName: "OPR.Hotels", operation: OperationType.Update, fieldName: "Id", ID: model.Id, parameters: hotelParameters);

            return id;
        }
    }
}
