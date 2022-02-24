using CustomerAPI.Application;
using CustomerAPI.Core.ValueObjects.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.RabbitMQ;
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
        #region [ Properties / Fields ]

        private readonly MvpRabbitMQClient rabbitMQClient;

        #endregion

        #region [ Ctor ]

        /// <summary>
        /// 
        /// </summary>
        public CustomerController(MvpRabbitMQClient rabbitMQClient)
        {
            this.rabbitMQClient = rabbitMQClient;
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
            var result = await FacadeService.CustomerService.GetBy(filter, pagingCriteria.ToPagingCriteria(), cancellationToken: cancellationToken);
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
            var result = await FacadeService.CustomerService.GetById(id, cancellationToken: cancellationToken);
            if (result.HasData())
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        /// <summary>
        /// Create customer
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ActionResult<string>), StatusCodes.Status201Created)]
        [Route("", Name = "CustomerCreate")]
        public ActionResult Create([FromBody] CustomerCreate model)
        {
            string token = rabbitMQClient.Publish(model, typeof(CustomerCreate).Name);
            return Created(nameof(Create), token);
        }

        /// <summary>
        /// Update customer data
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ActionResult<string>), StatusCodes.Status201Created)]
        [Route("{id}", Name = "CustomerUpdate")]
        public ActionResult Update(int id, [FromBody] CustomerUpdate model)
        {
            model.Id = id;
            string token = rabbitMQClient.Publish(model, typeof(CustomerUpdate).Name);
            return Created(nameof(Create), token);
        }

        /// <summary>
        /// Delete customer
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ActionResult<string>), StatusCodes.Status201Created)]
        [Route("{id}", Name = "CustomerDelete")]
        public ActionResult Delete(int id)
        {
            var model = new CustomerDelete() { Id = id };
            string token = rabbitMQClient.Publish(model, typeof(CustomerDelete).Name);
            return Created(nameof(Create), token);
        }

        #endregion
    }
}
