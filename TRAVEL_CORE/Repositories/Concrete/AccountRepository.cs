using System.Data.SqlClient;
using System.Data;
using TRAVEL_CORE.Tools;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities.Login;
using TRAVEL_CORE.Repositories.Abstract;

namespace TRAVEL_CORE.Repositories.Concrete
{
    public class AccountRepository : IAccountRepository
    {
        Connection connection = new Connection();
        public User AuthenticateUser(UserLogin user)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("username", user.UserName));
            parameters.Add(new SqlParameter("password", user.Password));
            parameters.Add(new SqlParameter("encryptedPassword", CommonTools.GetSha256Hash(user.Password)));

            var reader = connection.RunQuery(commandText: "SP_AUTHENTICATEUSER", parameters: parameters, commandType: CommandType.StoredProcedure);

            if (reader.Read())
            {
                User userData = new User();
                userData.UserId = Convert.ToInt32(reader["Id"]);
                userData.UserType = Convert.ToInt32(reader["Type"]);
                userData.FirstName = reader["Name"].ToString();
                userData.LastName = reader["Surname"].ToString();
                userData.UserName = reader["Username"].ToString();
                userData.Email = reader["Email"].ToString();
                userData.PhoneNo = reader["Tel"].ToString();
                userData.ChangePassword = Convert.ToInt32(reader["ChangePass"].ToString());
                return userData;
            }

            return null;
        }
    }
}
