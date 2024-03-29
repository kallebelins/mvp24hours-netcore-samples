﻿using CustomerAPI.Application;
using CustomerAPI.Core.ValueObjects.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Extensions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.WebAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        #region [ Fields ]
        private readonly FacadeService facade;
        #endregion

        #region [ Ctors ]
        public CustomerController(FacadeService facade)
        {
            this.facade = facade;
        }
        #endregion

        #region [ Actions / Resources ]

        /// <summary>
        /// Get paginated list of customers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IPagingResult<IList<CustomerResult>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IPagingResult<IList<CustomerResult>>>), StatusCodes.Status404NotFound)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult<IPagingResult<IList<CustomerResult>>>> GetBy([FromQuery] CustomerQuery filter, [FromQuery] PagingCriteriaRequest pagingCriteria, CancellationToken cancellationToken)
        {
            var result = await facade.CustomerService.GetBy(filter, pagingCriteria.ToPagingCriteria(), cancellationToken: cancellationToken);
            if (result.HasData())
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        /// <summary>
        /// Get customer with contact list
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<CustomerIdResult>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<CustomerIdResult>>), StatusCodes.Status404NotFound)]
        [Route("{id}", Name = "CustomerGetById")]
        public async Task<ActionResult<IBusinessResult<CustomerIdResult>>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await facade.CustomerService.GetById(id, cancellationToken: cancellationToken);
            if (result.HasData())
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        /// <summary>
        /// Run data seed customer
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status400BadRequest)]
        [Route("RunDataSeed", Name = "CustomerRunDataSeed")]
        public async Task<ActionResult<IBusinessResult<int>>> Create(CancellationToken cancellationToken)
        {
            var result = await facade.CustomerService.RunDataSeed(cancellationToken: cancellationToken);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }
            return Created(nameof(Create), result);
        }

        #endregion
    }
}
