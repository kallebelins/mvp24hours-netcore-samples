using CustomerAPI.Application;
using CustomerAPI.Core.ValueObjects.Contacts;
using CustomerAPI.Core.ValueObjects.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Core.Extensions;
using Mvp24Hours.Infrastructure.Extensions;
using Mvp24Hours.WebAPI.Controller;
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
    //[JWTAuthorize]
    public class CustomerController : BaseMvpController
    {
        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IPagingResult<IList<GetByCustomerResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult<IPagingResult<IList<GetByCustomerResponse>>>> GetBy([FromQuery] GetByCustomerRequest filter, [FromQuery] PagingCriteriaRequest clause)
        {
            var result = await FacadeService.CustomerService.GetBy(filter, clause.ToPagingService());
            return result.Data.AnyOrNotNull() ? Ok(result) : NotFound();
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IBusinessResult<GetByIdCustomerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}", Name = "CustomerGetById")]
        public async Task<ActionResult<IBusinessResult<GetByIdCustomerResponse>>> GetById(int id)
        {
            var result = await FacadeService.CustomerService.GetById(id);
            return result.Data == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IBusinessResult<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("", Name = "CustomerCreate")]
        public async Task<ActionResult<IBusinessResult<int>>> Create([FromBody] CreateCustomerRequest dto, CancellationToken cancellationToken)
        {
            var result = await FacadeService.CustomerService.Create(dto, cancellationToken);
            return result.HasErrors ? BadRequest(result) : Created(nameof(Create), result);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(IBusinessResult<VoidResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id}", Name = "CustomerUpdate")]
        public async Task<ActionResult<IBusinessResult<VoidResult>>> Update(int id, [FromBody] UpdateCustomerRequest dto, CancellationToken cancellationToken)
        {
            var result = await FacadeService.CustomerService.Update(id, dto, cancellationToken);
            return result.HasErrors ? BadRequest(result) : Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(IBusinessResult<VoidResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id}", Name = "CustomerDelete")]
        public async Task<ActionResult<IBusinessResult<VoidResult>>> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await FacadeService.CustomerService.Delete(id, cancellationToken);
            return result.HasErrors ? BadRequest(result) : Ok(result);
        }

    }
}
