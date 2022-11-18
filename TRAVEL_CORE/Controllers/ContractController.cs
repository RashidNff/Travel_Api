using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Contract;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Repositories.Abstract;
using TRAVEL_CORE.Repositories.Concrete;
using TRAVEL_CORE.Tools;

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
        public IActionResult GetContractBrowseData(FilterParameter filterParameter)
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

        [HttpGet]
        public IActionResult GetContractNo()

        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_contractRepository.GetContractNo()));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        [HttpPost]
        public IActionResult SaveContract(ContractData contract)
        {
            contract.CreatedBy = CommonTools.GetUserId(User.Claims.ToList());
            ResponseModel model = new();

            try
            {
                model = _contractRepository.SaveContract(contract);
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel { Message = "Unexpected error occurred!", Status = false});
            }

            return Ok(model);
        }

        [HttpGet]
        public IActionResult GetContractById(int contractId)
        {
            try
            {
                return Ok(_contractRepository.GetContractById(contractId));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        /// <summary>
        /// Change Contract Status by Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ChangeStatus(ChangeStatus model)
        {
            ResponseModel responseModel = new();
            try
            {
                responseModel = _contractRepository.ChangeStatus(model);
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel { Message = "Unexpected error occurred!", Status = false });
            }
            return Ok(model);
        }

    }
}
