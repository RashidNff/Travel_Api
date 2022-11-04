﻿using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities.Login;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Entities.Order.GetById;
using TRAVEL_CORE.Entities.TemplateCost;
using TRAVEL_CORE.Repositories.Abstract;
using TRAVEL_CORE.Tools;

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class TemplateCostRepository: ITemplateCostRepository
    {
        Connection connection = new Connection();

        public DataTable GetTemplateCostBrowseData(TempCostFilterParametr filterParameter)
        {
            string stringFilter = "";
            if (filterParameter.Filters != null)
            {
                foreach (var filter in filterParameter.Filters)
                {
                    stringFilter += $"and {filter.Key} Like N'%{filter.Value}%'";
                }
            }

            var query = $@"Select T.Id, T.Name, Vender, S.Value1 VenderService, Qty, VenderUnitPrice, VenderAmount, SaleUnitPrice, SaleAmount, Vat, S2.Value1 Currency, Round(CurrencyRate,4) CurrencyRate, CurrencyAmount from OPR.TemplateCost T
                            LEFT JOIN OPR.TemplateCostLines TL ON TL.TemplateCostId = T.Id
                            JOIN OBJ.SpeCodes S ON S.Refid = VenderService and S.Type = 'VenderService' and S.Status = 1
                            JOIN OBJ.SpeCodes S2 ON S2.Refid = Currency and S2.Type = 'Currency' and S2.Status = 1
                            Where T.Status = 1 {stringFilter}";

            var data = connection.GetData(commandText: query);
            return data;
        }

        public TemplateCost GetTemplateCostById(int templateCostId)
        {
            TemplateCost templateCosts = new();

            List<SqlParameter> templateCostParameters = new List<SqlParameter>();
            templateCostParameters.Add(new SqlParameter("Id", templateCostId));
            var reader = connection.RunQuery(commandText: "CRD.SP_GetTemplateCostById", parameters: templateCostParameters, commandType: CommandType.StoredProcedure);
            if (reader.Read())
            {
                templateCosts.Id = Convert.ToInt32(reader["Id"]);
                templateCosts.Name = reader["Name"].ToString();
            }
            reader.Close();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Id", templateCostId));

            var costlines = connection.GetData(commandText: "CRD.SP_GetTemplateCostLinesById", parameters: parameters, commandType: CommandType.StoredProcedure);
            templateCosts.templateCostLines = JsonConvert.DeserializeObject<List<TemplateCostLine>>(JsonConvert.SerializeObject(costlines));

            return templateCosts;
        }

        public int SaveTemplateCost(TemplateCost templateCosts)
        {
            int generatedId = 0;
            List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("Name", templateCosts.Name),
                    new SqlParameter("CreatedBy", templateCosts.CreateBy)
                };

            if (templateCosts.Id != 0)
                generatedId = connection.Execute(tableName: "OPR.TemplateCost", operation: OperationType.Update, fieldName: "Id", ID: templateCosts.Id, parameters: parameters);
            else
                generatedId = connection.Execute(tableName: "OPR.TemplateCost", operation: OperationType.Insert, parameters: parameters);

            DeleteTemplateCostLines(generatedId);
            SaveTemplateCostLines(templateCosts.templateCostLines, generatedId);

            return generatedId;
        }

        private void DeleteTemplateCostLines(int templateCostId)
        {
            connection.Execute(tableName: "OPR.TemplateCostLines", operation: OperationType.Delete, fieldName: "TemplateCostId", ID: templateCostId);
        }

        private void SaveTemplateCostLines(List<TemplateCostLine>? templateCostLines, int templateCostId)
        {
            foreach (var templateCost in templateCostLines)
            {
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("TemplateCostId", templateCostId),
                    new SqlParameter("Vender", templateCost.Vender),
                    new SqlParameter("VenderService", templateCost.VenderService),
                    new SqlParameter("Qty", templateCost.Qty),
                    new SqlParameter("VenderUnitPrice", templateCost.VenderUnitPrice),
                    new SqlParameter("VenderAmount", templateCost.VenderAmount),
                    new SqlParameter("SaleUnitPrice", templateCost.SaleUnitPrice),
                    new SqlParameter("SaleAmount", templateCost.SaleAmount),
                    new SqlParameter("VAT", templateCost.VAT),
                    new SqlParameter("Currency", templateCost.Currency),
                    new SqlParameter("CurrencyRate", templateCost.CurrencyRate),
                    new SqlParameter("CurrencyAmount", templateCost.CurrencyAmount)
                };

                connection.Execute(tableName: "OPR.TemplateCostLines", operation: OperationType.Insert, parameters: parameters);
            }
        }

        public void ChangeTemplateCostStatus(int templateCostId, int status)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TableName", "OPR.TemplateCost"));
            parameters.Add(new SqlParameter("Id", templateCostId));
            parameters.Add(new SqlParameter("Status", status));
            connection.RunQuery(commandText: "SP_CHANGESTATUS", parameters: parameters, commandType: CommandType.StoredProcedure);
        }
    }
}