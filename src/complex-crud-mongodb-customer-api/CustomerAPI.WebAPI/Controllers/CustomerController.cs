using CustomerAPI.Application;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Extensions;
using System.Collections.Generic;
using System.Net;
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
        public async Task<ActionResult<IBusinessResult<CustomerIdResult>>> GetById(string id, CancellationToken cancellationToken)
        {
            var result = await facade.CustomerService.GetById(id, cancellationToken: cancellationToken);
            if (result.HasData())
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        /// <summary>
        /// Create customer - allows you to send contacts
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<string>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<string>>), StatusCodes.Status400BadRequest)]
        [Route("", Name = "CustomerCreate")]
        public async Task<ActionResult<IBusinessResult<string>>> Create([FromBody] CustomerCreate model, CancellationToken cancellationToken)
        {
            var result = await facade.CustomerService.Create(model, cancellationToken: cancellationToken);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }
            return Created(nameof(Create), result);
        }

        /// <summary>
        /// Update customer data
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<VoidResult>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<VoidResult>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [Route("{id}", Name = "CustomerUpdate")]
        public async Task<ActionResult<IBusinessResult<int>>> Update(string id, [FromBody] CustomerUpdate model, CancellationToken cancellationToken)
        {
            var result = await facade.CustomerService.Update(id, model, cancellationToken: cancellationToken);
            if (result.HasErrors)
            {
                if (result.HasMessageKey(nameof(Messages.RECORD_NOT_FOUND_FOR_ID)))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            else if (result.GetDataValue() == 0)
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete customer
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status400BadRequest)]
        [Route("{id}", Name = "CustomerDelete")]
        public async Task<ActionResult<IBusinessResult<Customer>>> Delete(string id, CancellationToken cancellationToken)
        {
            var result = await facade.CustomerService.Delete(id, cancellationToken: cancellationToken);
            if (result.HasErrors)
            {
                if (result.HasMessageKey(nameof(Messages.RECORD_NOT_FOUND_FOR_ID)))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            return Ok(result);
        }

        #endregion
    }
}
