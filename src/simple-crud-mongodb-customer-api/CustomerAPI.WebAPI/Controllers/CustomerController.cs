using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Extensions;
using Mvp24Hours.Helpers;
using Mvp24Hours.WebAPI.Controller;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        private IRepositoryAsync<Customer> repository
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
        public CustomerController(IUnitOfWorkAsync uoW)
        {
            unitOfWork = uoW;
        }

        #endregion

        #region [ Actions / Resources ]

        /// <summary>
        /// Get paginated list of customers
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IPagingResult<IList<Customer>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<Customer>>>), StatusCodes.Status404NotFound)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult<IPagingResult<IList<Customer>>>> GetBy([FromQuery] CustomerFilter model, [FromQuery] PagingCriteriaRequest pagingCriteria, CancellationToken cancellationToken)
        {
            // construct expression to apply filter on database
            Expression<Func<Customer, bool>> clause = x => true;

            if (model.Name.HasValue())
            {
                clause = clause.And(x => x.Name.Contains(model.Name));
            }
            if (model.Active.HasValue)
            {
                clause = clause.And(x => x.Active == model.Active);
            }

            // checks if there are any records in the database from the filter
            if (!await repository.GetByAnyAsync(clause, cancellationToken))
            {
                // reply with standard message for record not found
                return NotFound(Messages.RECORD_NOT_FOUND
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<IList<Customer>>());
            }

            // apply filter with pagination
            var result = await repository.ToBusinessPagingAsync(clause, pagingCriteria.ToPagingCriteria());
            return Ok(result);
        }

        /// <summary>
        /// Get customer with contact list
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Customer>>), StatusCodes.Status404NotFound)]
        [Route("{id}", Name = "CustomerGetById")]
        public async Task<ActionResult<IBusinessResult<Customer>>> GetById(string id, CancellationToken cancellationToken)
        {
            // try to retrieve identifier with navigation property
            var model = await repository.GetByIdAsync(id, cancellationToken);
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
            if (model.Validate())
            {
                model.Created = TimeZoneHelper.GetTimeZoneNow();
                await repository.AddAsync(model, cancellationToken);
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
        public async Task<ActionResult<IBusinessResult<Customer>>> Update(string id, [FromBody] Customer model, CancellationToken cancellationToken)
        {
            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (model.Validate())
            {
                // gets entity through the identifier informed in the resource
                var modelDb = await repository.GetByIdAsync(id, cancellationToken);
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
                await repository.ModifyAsync(model, cancellationToken);
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
        public async Task<ActionResult<IBusinessResult<Customer>>> Delete(string id, CancellationToken cancellationToken)
        {
            // try to retrieve entity by identifier
            var model = await repository.GetByIdAsync(id, cancellationToken);
            if (model == null)
            {
                return NotFound(Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<Customer>());
            }

            // perform delete action
            await repository.RemoveAsync(model, cancellationToken);
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
