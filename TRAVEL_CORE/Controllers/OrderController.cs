﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TRAVEL_CORE.Entities;
using TRAVEL_CORE.Tools;
using TRAVEL_CORE.Entities.Order;
using TRAVEL_CORE.Repositories.Abstract;
using System.Net.Mail;
using System.Net;
using TRAVEL_CORE.Entities.Login;
using TRAVEL_CORE.Repositories.Concrete;

namespace TRAVEL_CORE.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Send Date for get Order Data
        /// </summary>
        /// <param name="filterParameter"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetOrderBrowseData(FilterParameter filterParameter)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_orderRepository.GetOrderBrowseData(filterParameter)));
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
                orderId = _orderRepository.SaveOrder(order);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }

            return Ok(new { orderId = orderId });
        }

        [HttpGet]
        public IActionResult GetOrderById(int orderId)
        {
            try
            {
                return Ok(_orderRepository.GetOrderById(orderId));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        [HttpGet]
        public IActionResult GetOrderNo()
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(_orderRepository.GetOrderNo()));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }

        /// <summary>
        /// Send Temaplate Cost Id to get Template Cost Data
        /// </summary>
        /// <param name="templateCost"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetTemplateCostData(int templateCostId)
        {
            try
            {
                return Ok(_orderRepository.GetTemplateCostData(templateCostId));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
        }


        /// <summary>
        /// Change Order Status by Id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ChangeOrderStatus(ChangeStatus model)
        {
            try
            {
                _orderRepository.ChangeOrderStatus(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
            return NoContent();
        }

        [HttpGet]
        public IActionResult SendMail()
        {
            try
            {
                _orderRepository.SendMail();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
            return NoContent();
        }


    }
}
