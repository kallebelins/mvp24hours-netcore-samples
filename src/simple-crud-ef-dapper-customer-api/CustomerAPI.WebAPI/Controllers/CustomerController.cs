using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Infrastructure.Extensions;
using CustomerAPI.WebAPI.Models;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.Infrastructure.Contexts;
using Mvp24Hours.Core.Contract.Infrastructure.Logging;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Extensions;
using Mvp24Hours.WebAPI.Controller;
using System.Collections.Generic;
using System.Linq;
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
    public class CustomerController : BaseMvpController
    {
        #region [ Fields / Properties ]

        private readonly IUnitOfWorkAsync unitOfWork;
        private readonly IValidator<Customer> validator;

        /// <summary>
        /// 
        /// </summary>
        protected IRepositoryAsync<Customer> Repository
        {
            get
            {
                return unitOfWork.GetRepository<Customer>();
            }
        }

        #endregion

        #region [ Ctors ]

        /// <summary>
        /// 
        /// </summary>
        public CustomerController(IUnitOfWorkAsync uoW, ILoggingService logging, INotificationContext notification, IValidator<Customer> validator)
            : base(logging, notification)
        {
            this.unitOfWork = uoW;
            this.validator = validator;
        }

        #endregion

        #region [ Actions / Resources ]

        /// <summary>
        /// Get paginated list of customers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IPagingResult<IEnumerable<Customer>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IEnumerable<Customer>>>), StatusCodes.Status404NotFound)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult<IPagingResult<IEnumerable<Customer>>>> GetBy([FromQuery] CustomerQuery model, [FromQuery] PagingCriteriaRequest pagingCriteria)
        {
            string whereSql = "(@Active is null or Active = @Active) and (@Name is null or Name like CONCAT('%',@Name,'%'))";

            var result = await unitOfWork
                .GetConnection()
                .QueryPagingResultAsync<Customer>(pagingCriteria, whereSql, new { model.Name, model.Active });

            // checks if there are any records in the database from the filter
            if (result == null || result.Summary.TotalCount == 0)
            {
                // reply with standard message for record not found
                return NotFound(Messages.RECORD_NOT_FOUND
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<IEnumerable<Customer>>());
            }

            return Ok(result);
        }

        /// <summary>
        /// Get customer with contact list
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status404NotFound)]
        [Route("{id}", Name = "CustomerGetById")]
        public async Task<ActionResult<IBusinessResult<Customer>>> GetById(int id)
        {
            // create projection for customer and contact by customer id
            string query = @"
                select * from Customer where Id = @id;
                select * from Contact where CustomerId = @id;
            ";

            Customer model = null;

            // apply multiple queries to load customer and contacts
            using (var result = await unitOfWork
                .GetConnection()
                .QueryMultipleAsync(query, new { id }))
            {
                model = await result.ReadFirstOrDefaultAsync<Customer>();
                if (model != null)
                    model.Contacts = (await result.ReadAsync<Contact>()).ToList();
            }

            if (model == null)
            {
                // reply with standard message for record not found
                return NotFound(Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<Customer>());
            }

            return Ok(model.ToBusiness());
        }

        /// <summary>
        /// Create customer - allows you to send contacts
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status400BadRequest)]
        [Route("", Name = "CustomerCreate")]
        public async Task<ActionResult<IBusinessResult<Customer>>> Create([FromBody] Customer model, CancellationToken cancellationToken)
        {
            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (model.Validate(NotificationContext, validator))
            {
                await Repository.AddAsync(model, cancellationToken);
                if (await unitOfWork.SaveChangesAsync(cancellationToken) > 0)
                {
                    return Created(nameof(Create), model.ToBusiness());
                }
            }
            // get message in request context, if not, use default message
            return BadRequest(NotificationContext
                .ToBusiness<Customer>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error))
            );
        }

        /// <summary>
        /// Update customer data
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [Route("{id}", Name = "CustomerUpdate")]
        public async Task<ActionResult<IBusinessResult<Customer>>> Update(int id, [FromBody] Customer model, CancellationToken cancellationToken)
        {
            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (model.Validate(NotificationContext, validator))
            {
                // gets entity through the identifier informed in the resource
                var modelDb = await Repository.GetByIdAsync(id, cancellationToken);
                if (modelDb == null)
                {
                    return NotFound(Messages.RECORD_NOT_FOUND_FOR_ID
                        .ToMessageResult(MessageType.Error)
                            .ToBusiness<Customer>());
                }

                // properties that cannot be overridden
                model.Id = modelDb.Id;
                model.Created = modelDb.Created;

                // apply changes to database
                await Repository.ModifyAsync(model, cancellationToken);
                if (await unitOfWork.SaveChangesAsync(cancellationToken) == 0)
                {
                    return StatusCode((int)HttpStatusCode.NotModified);
                }
                else
                {
                    return Created(nameof(Update), model.ToBusiness(Messages.OPERATION_SUCCESS.ToMessageResult(MessageType.Success)));
                }
            }
            // get message in request context, if not, use default message
            return BadRequest(NotificationContext
                .ToBusiness<Customer>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error))
            );
        }

        /// <summary>
        /// Delete customer
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status400BadRequest)]
        [Route("{id}", Name = "CustomerDelete")]
        public async Task<ActionResult<IBusinessResult<Customer>>> Delete(int id, CancellationToken cancellationToken)
        {
            // try to retrieve entity by identifier
            var model = await Repository.GetByIdAsync(id, cancellationToken);
            if (model == null)
            {
                return NotFound(Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<Customer>());
            }

            // perform delete action
            await Repository.RemoveAsync(model, cancellationToken);
            if (await unitOfWork.SaveChangesAsync(cancellationToken) > 0)
            {
                return Ok(Messages.OPERATION_SUCCESS
                    .ToMessageResult(MessageType.Success)
                        .ToBusiness<Customer>());
            }
            // get message in request context, if not, use default message
            return BadRequest(NotificationContext
                .ToBusiness<Customer>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error))
            );
        }

        #endregion
    }
}
