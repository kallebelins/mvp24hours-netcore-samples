using CustomerAPI.Application;
using CustomerAPI.Core.ValueObjects.Customers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.RabbitMQ.Core.Contract;
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
    public class CustomerController : BaseMvpController
    {
        #region [ Properties / Fields ]

        private readonly IMvpRabbitMQProducer<CreateCustomerRequest> createProducer;
        private readonly IMvpRabbitMQProducer<UpdateCustomerRequest> updateProducer;
        private readonly IMvpRabbitMQProducer<DeleteCustomerRequest> deteleProducer;

        #endregion

        #region [ Ctor ]

        /// <summary>
        /// 
        /// </summary>
        public CustomerController(
            IMvpRabbitMQProducer<CreateCustomerRequest> createProducer,
            IMvpRabbitMQProducer<UpdateCustomerRequest> updateProducer,
            IMvpRabbitMQProducer<DeleteCustomerRequest> deteleProducer)
        {
            this.createProducer = createProducer;
            this.updateProducer = updateProducer;
            this.deteleProducer = deteleProducer;
        }

        #endregion

        #region [ Actions / Resources ]

        /// <summary>
        /// Get paginated list of customers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IPagingResult<IList<GetByCustomerResponse>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IPagingResult<IList<GetByCustomerResponse>>>), StatusCodes.Status404NotFound)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult<IPagingResult<IList<GetByCustomerResponse>>>> GetBy([FromQuery] GetByCustomerRequest filter, [FromQuery] PagingCriteriaRequest pagingCriteria, CancellationToken cancellationToken)
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
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<GetByIdCustomerResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<GetByIdCustomerResponse>>), StatusCodes.Status404NotFound)]
        [Route("{id}", Name = "CustomerGetById")]
        public async Task<ActionResult<IBusinessResult<GetByIdCustomerResponse>>> GetById(int id, CancellationToken cancellationToken)
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
        public ActionResult Create([FromBody] CreateCustomerRequest model)
        {
            createProducer.Publish(model);
            return Created(nameof(Create), model.Token);
        }

        /// <summary>
        /// Update customer data
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ActionResult<string>), StatusCodes.Status201Created)]
        [Route("{id}", Name = "CustomerUpdate")]
        public ActionResult Update(int id, [FromBody] UpdateCustomerRequest model)
        {
            //model.Id = id;
            updateProducer.Publish(model);
            return Created(nameof(Create), model.Token);
        }

        /// <summary>
        /// Delete customer
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ActionResult<string>), StatusCodes.Status201Created)]
        [Route("{id}", Name = "CustomerDelete")]
        public ActionResult Delete(int id)
        {
            var model = new DeleteCustomerRequest() { Id = id };
            deteleProducer.Publish(model);
            return Created(nameof(Create), model.Token);
        }

        #endregion
    }
}
