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
    [Route("api/customer/[controller]")]
    [ApiController]
    //[JWTAuthorize]
    public class CustomerContactController : BaseMvpController
    {
        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IBusinessResult<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{customerId:int}/contact", Name = "ContactCreate")]
        public async Task<ActionResult<IBusinessResult<int>>> Create(int customerId, [FromBody] CreateContactRequest dto, CancellationToken cancellationToken)
        {
            var result = await FacadeService.CustomerContactService.Create(customerId, dto, cancellationToken);
            return result.HasErrors ? BadRequest(result) : Created(nameof(Create), result);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(IBusinessResult<VoidResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{customerId:int}/contact/{id}", Name = "ContactUpdate")]
        public async Task<ActionResult<IBusinessResult<VoidResult>>> Update(int customerId, int id, [FromBody] UpdateContactRequest dto, CancellationToken cancellationToken)
        {
            var result = await FacadeService.CustomerContactService.Update(customerId, id, dto, cancellationToken);
            return result.HasErrors ? BadRequest(result) : Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(IBusinessResult<VoidResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{customerId:int}/contact/{id}", Name = "ContactDelete")]
        public async Task<ActionResult<IBusinessResult<VoidResult>>> Delete(int customerId, int id, CancellationToken cancellationToken)
        {
            var result = await FacadeService.CustomerContactService.Delete(customerId, id, cancellationToken);
            return result.HasErrors ? BadRequest(result) : Ok(result);
        }

    }
}
