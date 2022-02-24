using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using CustomerAPI.WebAPI.Pipe.Operations.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
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
            var message = Pipeline.GetMessage();

            // checks for failure in the notification context
            if (message.IsFaulty)
            {
                return BadRequest(message.Messages.ToBusiness<IList<GetByCustomerResponse>>());
            }

            // get message content
            var result = message.GetContent<List<GetByCustomerResponse>>();

            // checks if there are any records
            if (!result.AnySafe())
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
            var message = Pipeline.GetMessage();

            // checks for failure in the notification context
            if (message.IsFaulty)
            {
                return BadRequest(message.Messages.ToBusiness<GetByIdCustomerResponse>());
            }

            // get message content
            var result = message.GetContent<GetByIdCustomerResponse>();

            if (result == null)
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
