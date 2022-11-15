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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult SavePerson(PersonData person)
        {
            person.CreatedBy = CommonTools.GetUserId(User.Claims.ToList());
            int personId = 0;

            try
            {
                personId = _personRepository.SavePerson(person);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }

            return Ok(new { personId = personId });
        }

        [AllowAnonymous]
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
        public IActionResult ChangeOrderStatus(ChangeStatus model)
        {
            try
            {
                _personRepository.ChangeOrderStatus(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
            return NoContent();
        }

    }
}
