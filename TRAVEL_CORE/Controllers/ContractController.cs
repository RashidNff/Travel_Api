﻿using Microsoft.AspNetCore.Authorization;
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
        public IActionResult SaveContract(SaveContract contract)
        {
            contract.CreatedBy = CommonTools.GetUserId(User.Claims.ToList());
            int contractId = 0;

            try
            {
                contractId = _contractRepository.SaveContract(contract);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }

            return Ok(new { contractId = contractId });
        }

        /// <summary>
        /// Change Contract Status by Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult ChangeOrderStatus(ChangeStatus model)
        {
            try
            {
                _contractRepository.ChangeOrderStatus(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Unexpected error occurred!" });
            }
            return NoContent();
        }

    }
}