using Microsoft.OpenApi.Models;
using System.Data.SqlClient;
using System.Data;
using TMTM2_Web_Api.Tools;

namespace TRAVEL_CORE.DAL
{
    public class Connection
    {

        public SqlDataReader RunQuery(string commandText, List<SqlParameter> parameters = null, CommandType commandType = CommandType.Text, string connectionString = null)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(commandText, OpenConnection(connectionString));
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }
                cmd.CommandTimeout = 1000;
                cmd.CommandType = commandType;
                SqlDataReader reader = cmd.ExecuteReader();
                CloseConnetion(connectionString);
                cmd.Parameters.Clear();
                return reader;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public DataTable GetData(string commandText, List<SqlParameter> parameters = null, CommandType commandType = CommandType.Text, string connectionString = null)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(commandText, OpenConnection(connectionString));
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }
                cmd.CommandTimeout = 1000;
                cmd.CommandType = commandType;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                adp.Dispose();
                cmd.Dispose();
                CloseConnetion();
                cmd.Parameters.Clear();
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public int Execute(string tableName, OperationType operation, string fieldName = "", int ID = 0, List<SqlParameter> parameters = null, string connectionString = null, bool transaction = false)
        {
            string query = "";
            string fields = "";
            string pfields = "";

            if (operation == OperationType.Insert)
            {
                foreach (var data in parameters)
                {
                    fields += data.ParameterName + ",";
                    pfields += "@" + data.ParameterName + ",";
                }
                fields = fields.Substring(0, fields.Length - 1);
                pfields = pfields.Substring(0, pfields.Length - 1);

                query = @"INSERT into " + tableName + "(" + fields + ")";
                query += @"VALUES (" + pfields + ");select SCOPE_IDENTITY() OID;";
            }
            else if (operation == OperationType.Update)
            {
                if (parameters != null)
                {
                    foreach (var data in parameters)
                    {
                        fields += data.ParameterName + "=@" + data.ParameterName + ",";
                    }
                    fields = fields.Substring(0, fields.Length - 1);

                    query = @"UPDATE  " + tableName +
                             " SET " + fields;
                    query += @" where (" + fieldName + "=" + ID + ");";
                }
                else
                {
                    return 0;
                }
            }


            if (operation == OperationType.Delete)
            {
                query = @"Delete FROM " + tableName + "  WHERE " + fieldName + "=" + ID;
            }

            using (SqlConnection con = new SqlConnection(connectionString ?? CommonTools.GetAppSetttigs("ConnectionStrings:default")))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    try
                    {
                        if (transaction)
                        {
                            command.Transaction = con.BeginTransaction();

                            foreach (var data in parameters)
                            {
                                command.Parameters.AddWithValue(data.ParameterName, data.Value);
                            }

                            var sqlreader = command.ExecuteReader();

                            if (sqlreader.Read())
                            {
                                ID = int.Parse(sqlreader["OID"].ToString());
                            }

                            sqlreader.Close();
                            command.Transaction.Commit();
                            command.Connection.Close();
                            command.Dispose();
                        }
                        else
                        {
                            if (operation != OperationType.Delete)
                            {
                                foreach (var data in parameters)
                                {
                                    command.Parameters.AddWithValue(data.ParameterName, data.Value);
                                }
                            }

                            var sqlreader = command.ExecuteReader();

                            if (sqlreader.Read())
                            {
                                ID = int.Parse(sqlreader["OID"].ToString());
                            }
                            sqlreader.Close();
                            command.Connection.Close();
                            command.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (transaction)
                            command.Transaction.Rollback();
                    }
                }
                con.Close();
            }

            return ID;
        }


        public SqlConnection OpenConnection(string connectionString = null)
        {
            SqlConnection con = new SqlConnection(connectionString ?? CommonTools.GetAppSetttigs("ConnectionStrings:default"));
            SqlConnection.ClearAllPools();
            con.Open();
            return con;
        }


        public void CloseConnetion(string connectionString = null)
        {
            SqlConnection con = new SqlConnection(connectionString ?? CommonTools.GetAppSetttigs("ConnectionStrings:default"));
            con.Close();
        }

    }
}
