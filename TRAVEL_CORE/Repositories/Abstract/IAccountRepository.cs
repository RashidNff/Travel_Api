using TRAVEL_CORE.Entities.Login;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface IAccountRepository
    {
        User AuthenticateUser(UserLogin user);
    }
}
