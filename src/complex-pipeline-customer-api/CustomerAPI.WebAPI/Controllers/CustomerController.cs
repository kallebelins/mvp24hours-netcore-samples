using CustomerAPI.Application;
using CustomerAPI.Core.ValueObjects.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Extensions;
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
        /// Get list of customers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<CustomerResult>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<CustomerResult>>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<CustomerResult>>>), StatusCodes.Status400BadRequest)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult<IBusinessResult<IList<CustomerResult>>>> GetBy([FromQuery] CustomerQuery model)
        {
            var result = await facade.CustomerService.GetBy(model);
            // checks for failure in the notification context
            if (result.HasErrors)
            {
                return BadRequest(result);
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
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<CustomerIdResult>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<CustomerIdResult>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<CustomerIdResult>>), StatusCodes.Status400BadRequest)]
        [Route("{id}", Name = "CustomerGetById")]
        public async Task<ActionResult<IBusinessResult<CustomerIdResult>>> GetById(int id)
        {
            var result = await facade.CustomerService.GetById(id);
            // checks for failure in the notification context
            if (result.HasErrors)
            {
                return BadRequest(result);
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
