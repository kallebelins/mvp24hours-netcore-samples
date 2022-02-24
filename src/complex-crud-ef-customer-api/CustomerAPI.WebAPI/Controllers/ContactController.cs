using CustomerAPI.Application;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.ValueObjects.Contacts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
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
    [Route("api/Customer")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        #region [ Actions / Resources ]

        /// <summary>
        /// Get paginated list of customers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<Contact>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<Contact>>>), StatusCodes.Status404NotFound)]
        [Route("{customerId:int}/Contact", Name = "ContactGetBy")]
        public async Task<ActionResult<IBusinessResult<IList<Contact>>>> GetBy(int customerId, CancellationToken cancellationToken)
        {
            var result = await FacadeService.ContactService.GetBy(customerId, cancellationToken: cancellationToken);
            if (result.HasData())
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        /// <summary>
        /// Create contact for customer
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status400BadRequest)]
        [Route("{customerId:int}/Contact", Name = "ContactCreate")]
        public async Task<ActionResult<IBusinessResult<IList<int>>>> Create(int customerId, [FromBody] ContactCreate model, CancellationToken cancellationToken)
        {
            var result = await FacadeService.ContactService.Create(customerId, model, cancellationToken: cancellationToken);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }
            return Created(nameof(Create), result);
        }

        /// <summary>
        /// Update customer contact
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [Route("{customerId:int}/Contact/{id}", Name = "ContactUpdate")]
        public async Task<ActionResult<IBusinessResult<IList<int>>>> Update(int customerId, int id, [FromBody] ContactUpdate model, CancellationToken cancellationToken)
        {
            var result = await FacadeService.ContactService.Update(customerId, id, model, cancellationToken: cancellationToken);
            if (result.HasErrors)
            {
                if (result.HasMessageKey("NotFound"))
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
        /// Delete customer contact
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<int>>), StatusCodes.Status400BadRequest)]
        [Route("{customerId:int}/Contact/{id}", Name = "ContactDelete")]
        public async Task<ActionResult<IBusinessResult<IList<int>>>> Delete(int customerId, int id, CancellationToken cancellationToken)
        {
            var result = await FacadeService.ContactService.Delete(customerId, id, cancellationToken: cancellationToken);
            if (result.HasErrors)
            {
                if (result.HasMessageKey("NotFound"))
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
