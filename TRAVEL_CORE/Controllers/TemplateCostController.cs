using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Entities.TemplateCost;
using TRAVEL_CORE.Repositories.Abstract;
using TRAVEL_CORE.Tools;

namespace TRAVEL_CORE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class TemplateCostController : ControllerBase
    {
        private readonly ITemplateCostRepository _templateCostRepository;

        public TemplateCostController(ITemplateCostRepository templateCostRepository)
        {
            _templateCostRepository = templateCostRepository;
        }

        /// <summary>
        /// Get Template Cost browse data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetTemplateCostBrowseData()
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_templateCostRepository.GetTemplateCostBrowseData()));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        /// <summary>
        /// Get Expence Names by Id
        /// </summary>
        /// <param name="templateCostId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetExpences(int templateCostId)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_templateCostRepository.GetExpences(templateCostId)));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        /// <summary>
        /// Save Template Cost Data
        /// </summary>
        /// <param name="filterParameter"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveTemplateCost(TemplateCost templateCosts)
        {
            int operationId = 0;
            templateCosts.CreateBy = CommonTools.GetUserId(User.Claims.ToList());

            try
            {
                operationId = _templateCostRepository.SaveTemplateCost(templateCosts);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
            return Ok(new { operationId = operationId });
        }

        /// <summary>
        /// Get Template Cost Data by Id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetTemplateCostById(int templateCostId)
        {
            try
            {
                return Ok(_templateCostRepository.GetTemplateCostById(templateCostId));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        /// <summary>
        /// Change Template Cost Status by Id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ChangeTemplateCostStatus(ChangeStatus model)
        {
            try
            {
                _templateCostRepository.ChangeTemplateCostStatus(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
            return NoContent();
        }

    }
}
