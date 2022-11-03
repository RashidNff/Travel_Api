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
        /// Get Template Cost Data
        /// </summary>
        /// <param name="filterParameter"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetTemplateCostBrowseData(TempCostFilterParametr filterParametr)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_templateCostRepository.GetTemplateCostBrowseData(filterParametr)));
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        [HttpPut]
        public IActionResult ChangeTemplateCostStatus(int templateCostId, int status)
        {
            try
            {
                _templateCostRepository.ChangeTemplateCostStatus(templateCostId, status);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
            return NoContent();
        }

    }
}
