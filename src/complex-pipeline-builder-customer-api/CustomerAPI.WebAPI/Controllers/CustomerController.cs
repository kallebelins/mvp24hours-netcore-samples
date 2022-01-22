using CustomerAPI.Application;
using CustomerAPI.Core.ValueObjects.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Extensions;
using Mvp24Hours.WebAPI.Controller;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.WebAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseMvpController
    {
        #region [ Actions / Resources ]

        /// <summary>
        /// Get list of customers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<GetByCustomerResponse>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<GetByCustomerResponse>>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<GetByCustomerResponse>>>), StatusCodes.Status400BadRequest)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult<IBusinessResult<IList<GetByCustomerResponse>>>> GetBy([FromQuery] GetByCustomerFilterRequest model)
        {
            var result = await FacadeService.CustomerService.GetBy(model);
            // checks for failure in the notification context
            if (NotificationContext.HasErrorNotifications)
            {
                return BadRequest(NotificationContext.ToBusiness<IList<GetByCustomerResponse>>());
            }
            else if (result.HasData())
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Get customer with contact list
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<GetByIdCustomerResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<GetByIdCustomerResponse>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<GetByIdCustomerResponse>>), StatusCodes.Status400BadRequest)]
        [Route("{id}", Name = "CustomerGetById")]
        public async Task<ActionResult<IBusinessResult<GetByIdCustomerResponse>>> GetById(int id)
        {
            var result = await FacadeService.CustomerService.GetById(id);
            // checks for failure in the notification context
            if (NotificationContext.HasErrorNotifications)
            {
                return BadRequest(NotificationContext.ToBusiness<GetByIdCustomerResponse>());
            }
            else if (result.HasData())
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        #endregion
    }
}
