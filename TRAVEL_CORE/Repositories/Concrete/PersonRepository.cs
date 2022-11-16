using Microsoft.OpenApi.Models;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Firm;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Entities.Order.GetById;
using TRAVEL_CORE.Entities.Person;
using TRAVEL_CORE.Repositories.Abstract;
using TRAVEL_CORE.Tools;

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class PersonRepository : IPersonRepository
    {
        Connection connection = new Connection();

        public DataTable GetPersonBrowseData(FilterParameter filterParameter)
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
            parameters.Add(new SqlParameter("PersonStatus", filterParameter.OrderStatus));

            if (filterParameter.OrderStatus == 0)
                query = $@"Select P.Id, Name +' '+ Surname Fullname, S2.Value1 Gender, S3.Value1 DocType, DocNumber, DocExpireDate, DocScan,
                            CASE
	                            when P.Status = 3 then S.Value1
	                            else Cast(P.Status as nvarchar(20))
                            END Status,S.ColorCode
                            from CRD.PersonDetails P
                            Left Join  OBJ.SpeCodes S ON S.RefId = P.Status and S.Type = 'OrderStatus' and S.Status = 1
                            Left Join  OBJ.SpeCodes S2 ON S2.RefId = P.Gender and S2.Type = 'GenderType' and S2.Status = 1
                            Left Join  OBJ.SpeCodes S3 ON S3.RefId = P.DocType and S3.Type = 'DocType' and S3.Status = 1
                            WHERE P.CreatedDate between @FromDate and @ToDate {stringFilter}
                            Order by P.Id DESC";
            else
                query = $@"Select P.Id, Name +' '+ Surname Fullname, S2.Value1 Gender, S3.Value1 DocType, DocNumber, DocExpireDate, DocScan,
                            CASE
	                            when P.Status = 3 then S.Value1
	                            else Cast(P.Status as nvarchar(20))
                            END Status,S.ColorCode
                            from CRD.PersonDetails P
                            Left Join  OBJ.SpeCodes S ON S.RefId = P.Status and S.Type = 'OrderStatus' and S.Status = 1
                            Left Join  OBJ.SpeCodes S2 ON S2.RefId = P.Gender and S2.Type = 'GenderType' and S2.Status = 1
                            Left Join  OBJ.SpeCodes S3 ON S3.RefId = P.DocType and S3.Type = 'DocType' and S3.Status = 1
                            WHERE P.CreatedDate between @FromDate and @ToDate and P.Status = @PersonStatus {stringFilter}
                            Order by P.Id DESC";

            var data = connection.GetData(commandText: query, parameters: parameters);
            return data;
        }

        public PersonData GetPersonById(int personId)
        {
            PersonData person = new();

            List<SqlParameter> Parameters = new List<SqlParameter>();
            Parameters.Add(new SqlParameter("Id", personId));
            var reader = connection.RunQuery(commandText: "CRD.SP_GetPersonById", parameters: Parameters, commandType: CommandType.StoredProcedure);
            if (reader.Read())
            {
                person.Id = Convert.ToInt32(reader["Id"]);
                person.Name = reader["Name"].ToString();
                person.Surname = reader["Surname"].ToString();
                person.Gender = Convert.ToInt16(reader["Gender"].ToString());
                person.BirthDate = Convert.ToDateTime(reader["BirthDate"].ToString(), CultureInfo.CreateSpecificCulture("en-GB"));
                person.Phone = reader["Phone"].ToString();
                person.Email = reader["Email"].ToString();
                person.DocType = Convert.ToInt32(reader["DocType"].ToString());
                person.DocNumber = reader["DocNumber"].ToString();
                person.DocIssueCountry = reader["DocIssueCountry"].ToString();
                person.DocExpireDate = Convert.ToDateTime(reader["DocExpireDate"].ToString(), CultureInfo.CreateSpecificCulture("en-GB"));
                person.DocScan = reader["DocScan"].ToString();
            }
            reader.Close();

            return person;
        }

        public int SavePerson(PersonData savePerson)
        {
            int generatedId = 0;
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("Name", savePerson.Name),
                new SqlParameter("Surname", savePerson.Surname),
                new SqlParameter("Gender", savePerson.Gender),
                new SqlParameter("BirthDate", savePerson.BirthDate),
                new SqlParameter("Phone", savePerson.Phone),
                new SqlParameter("Email", savePerson.Email),
                new SqlParameter("DocType", savePerson.DocType),
                new SqlParameter("DocNumber", savePerson.DocNumber),
                new SqlParameter("DocIssueCountry", savePerson.DocIssueCountry),
                new SqlParameter("DocExpireDate", savePerson.DocExpireDate),
                new SqlParameter("CreatedBy", savePerson.CreatedBy)
            };

            if (!string.IsNullOrEmpty(savePerson.DocName))
            {
                FileOperation fileOperation = new FileOperation();
                UploadedFile uploaded = fileOperation.MoveFile(savePerson.DocName, "PersonDetail");
                parameters.Add(new SqlParameter("DocScan", uploaded.FilePath));
            }
            else
                parameters.Add(new SqlParameter("DocScan", savePerson.DocScan));


            List<SqlParameter> checkParameters = new List<SqlParameter>();
            checkParameters.Add(new SqlParameter("DocNumber", savePerson.DocNumber));

            var reader = connection.RunQuery(commandText: "CRD.SP_CheckPerson", parameters: checkParameters, commandType: CommandType.StoredProcedure);
            if (reader.Read())
                return 0;


            if (savePerson.Id != 0)
                generatedId = connection.Execute(tableName: "CRD.PersonDetails", operation: OperationType.Update, fieldName: "Id", ID: savePerson.Id, parameters: parameters);
            else
                generatedId = connection.Execute(tableName: "CRD.PersonDetails", operation: OperationType.Insert, parameters: parameters);

            return generatedId;
        }

        public void ChangeOrderStatus(ChangeStatus model)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TableName", "CRD.PersonDetails"));
            parameters.Add(new SqlParameter("Id", model.Id));
            parameters.Add(new SqlParameter("Status", model.Status));
            connection.RunQuery(commandText: "SP_CHANGESTATUS", parameters: parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
