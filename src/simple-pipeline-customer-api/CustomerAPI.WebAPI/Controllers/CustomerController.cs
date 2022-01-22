using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using CustomerAPI.WebAPI.Pipe.Operations.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
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
        #region [ Fields / Properties ]

        private readonly IPipelineAsync _pipeline;

        private IPipelineAsync Pipeline => _pipeline;

        #endregion

        #region [ Ctors ]

        /// <summary>
        /// 
        /// </summary>
        public CustomerController(IPipelineAsync pipeline)
        {
            _pipeline = pipeline;
        }

        #endregion

        #region [ Actions / Resources ]

        /// <summary>
        /// Get list of customers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<GetByCustomerResponse>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<GetByCustomerResponse>>>), StatusCodes.Status404NotFound)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult<IBusinessResult<IList<GetByCustomerResponse>>>> GetBy([FromQuery] GetByCustomerFilterRequest model)
        {
            // add operations/steps
            Pipeline.Add<GetCustomerClientStep>();
            Pipeline.Add<GetByCustomerMapperResponseStep>();

            // run pipeline with package with content (int -> id)
            await Pipeline.ExecuteAsync(model.ToMessage());

            // try to get response content
            var result = Pipeline.GetMessage()
                .GetContent<List<GetByCustomerResponse>>();

            // checks for failure in the notification context
            if (NotificationContext.HasErrorNotifications)
            {
                return BadRequest(NotificationContext.ToBusiness<IList<GetByCustomerResponse>>());
            }
            // checks if there are any records
            else if (!result.AnyOrNotNull())
            {
                // reply with standard message for record not found
                return NotFound(Messages.RECORD_NOT_FOUND
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<IList<GetByCustomerResponse>>());
            }
            return Ok(result.ToBusiness());
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
            // add operations/steps
            Pipeline.Add<GetCustomerClientStep>();
            Pipeline.Add<GetByIdCustomerMapperResponseStep>();

            // run pipeline with package with content (int -> id)
            await Pipeline.ExecuteAsync(id.ToMessage("id"));

            // try to get response content
            var result = Pipeline.GetMessage()
                .GetContent<GetByIdCustomerResponse>();

            // checks for failure in the notification context
            if (NotificationContext.HasErrorNotifications)
            {
                return BadRequest(NotificationContext.ToBusiness<GetByIdCustomerResponse>());
            }
            else if (result == null)
            {
                // reply with standard message for record not found
                return NotFound(Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<GetByIdCustomerResponse>());
            }
            return Ok(result.ToBusiness());
        }

        #endregion
    }
}
