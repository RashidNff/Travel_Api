using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMTM2_Web_Api.Entities;
using TMTM2_Web_Api.Tools;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Repositories.Abstract;

namespace TRAVEL_CORE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _airRepository;

        public OrderController(IOrderRepository airRepository)
        {
            _airRepository = airRepository;
        }

        /// <summary>
        /// Send Date for get Airway Data
        /// </summary>
        /// <param name="filterParameter"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAirBrowseData(FilterParameter filterParameter)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_airRepository.GetAirBrowseData(filterParameter)));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }


        [HttpPost]
        public IActionResult SaveOrder(SaveOrder order)
        {
            order.CreatedBy = CommonTools.GetUserId(User.Claims.ToList());

            int orderId = 0;

            try
            {
                orderId = _airRepository.SaveOrder(order);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }

            return Ok(new { orderId = orderId });
        }

    }
}
