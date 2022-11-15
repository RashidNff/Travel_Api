using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Repositories.Abstract;

namespace TRAVEL_CORE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ContractController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;

        public ContractController(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }
        /// <summary>
        /// Send Date and Status for get Contract Data;
        /// 0 - All
        /// 1 - Active
        /// 2 - Deactive
        /// 3 - Deleted
        /// </summary>
        /// <param name="filterParameter"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetOrderBrowseData(FilterParameter filterParameter)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_contractRepository.GetContractBrowseData(filterParameter)));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }
    }
}
