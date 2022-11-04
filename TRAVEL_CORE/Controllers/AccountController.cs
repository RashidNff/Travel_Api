using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TRAVEL_CORE.Tools;
using TRAVEL_CORE.Entities.Login;
using TRAVEL_CORE.Repositories.Abstract;

namespace TRAVEL_CORE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }   

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Authenticate(UserLogin user)
        {
            User userData = new User();

            try
            {
                userData = _accountRepository.AuthenticateUser(user);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }

            if (userData == null)
                return BadRequest(new { message = "Username or password is incorrect." });

            userData.Token = CommonTools.GetJwt(userData.UserId, userData.UserName, user.RefreshToken ? 180 : 500); // if RefreshToken then set 180 minutes(Token refresh popup) otherwise 500 minutes(Login page)

            return Ok(userData);
        }


    }
}
