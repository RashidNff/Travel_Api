using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using TRAVEL_CORE.DAL;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.Firm;
using TRAVEL_CORE.Entities.Firm;
using TRAVEL_CORE.Repositories.Abstract;
using TRAVEL_CORE.Repositories.Concrete;
using TRAVEL_CORE.Tools;

namespace TRAVEL_CORE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class FirmController : ControllerBase
    {
        private readonly IFirmRepository _firmRepository;

        public FirmController(IFirmRepository firmRepository)
        {
            _firmRepository = firmRepository;
        }

        /// <summary>
        /// Send Date and Status for get Firm Data;
        /// 0 - All
        /// 1 - Active
        /// 2 - Deactive
        /// 3 - Deleted
        /// </summary>
        /// <param name="filterParameter"></param>
        /// <returns></returns>

        [HttpPost]
        public IActionResult GetFirmBrowseData(FilterParameter filterParameter)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_firmRepository.GetFirmBrowseData(filterParameter)));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        [HttpGet]
        public IActionResult GetFirmCode()

        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_firmRepository.GetFirmCode()));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        [HttpPost]
        public IActionResult SaveFirm(FirmData firm)
        {
            firm.CreatedBy = CommonTools.GetUserId(User.Claims.ToList());
            int firmId = 0;
            ResponseModel model = new();

            try
            {
                model = _firmRepository.SaveFirm(firm);
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel { Message = "Unexpected error occurred!", Status = false });
            }

            return Ok(model);
        }

        /// <summary>
        /// Send contract ID to get data
        /// </summary>
        /// <param name="firmId"></param>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetFirmById(int firmId)
        {
            try
            {
                return Ok(_firmRepository.GetFirmById(firmId));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        /// <summary>
        /// Change Firm Status by Id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="contractCheck"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ChangeStatus(ChangeStatus model, bool contractCheck)
        {
            ResponseModel responseModel = new();
            try
            {
                responseModel = _firmRepository.ChangeStatus(model, contractCheck);
            }
            catch (Exception)
            {
                return BadRequest(new ResponseModel { Message = "Unexpected error occurred!", Status = false });
            }
            return Ok(model);
        }
    }
}
