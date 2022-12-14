using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Firm;
using TRAVEL_CORE.Entities.Person;
using TRAVEL_CORE.Repositories.Abstract;
using TRAVEL_CORE.Repositories.Concrete;
using TRAVEL_CORE.Tools;

namespace TRAVEL_CORE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonRepository _personRepository;

        public PersonController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetFirmBrowseData(FilterParameter filterParameter)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_personRepository.GetPersonBrowseData(filterParameter)));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        [HttpPost]
        public IActionResult SavePerson(PersonData person)
        {
            person.CreatedBy = CommonTools.GetUserId(User.Claims.ToList());
            int personId = 0;
            ResponseModel model = new();

            try
            {
                model = _personRepository.SavePerson(person);
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel { Message = "Unexpected error occurred!", Status = false });
            }

            return Ok(model);
        }

        /// <summary>
        /// Send person ID to get data
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetPersonById(int personId)
        {
            try
            {
                return Ok(_personRepository.GetPersonById(personId));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        /// <summary>
        /// Change Person Status by Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ChangeStatus(ChangeStatus model)
        {
            ResponseModel responseModel = new();
            try
            {
                responseModel = _personRepository.ChangeStatus(model);
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel { Message = "Unexpected error occurred!", Status = false, Data = null });
            }
            return Ok(responseModel);
        }

    }
}
