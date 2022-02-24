using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
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
        #region [ Fields / Properties ]

        private readonly IUnitOfWorkAsync unitOfWork;
        private readonly IValidator<Contact> validator;

        /// <summary>
        /// 
        /// </summary>
        protected IRepositoryAsync<Contact> Repository
        {
            get
            {
                return unitOfWork.GetRepository<Contact>();
            }
        }

        #endregion

        #region [ Ctors ]

        /// <summary>
        /// 
        /// </summary>
        public ContactController(IUnitOfWorkAsync uoW, IValidator<Contact> validator)
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
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<Contact>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<IList<Contact>>>), StatusCodes.Status404NotFound)]
        [Route("{customerId:int}/Contact", Name = "ContactGetBy")]
        public async Task<ActionResult<IBusinessResult<IList<Contact>>>> GetBy(int customerId, CancellationToken cancellationToken)
        {
            // load all customer contacts by id
            var result = await Repository.GetByAsync(x => x.CustomerId == customerId, cancellationToken: cancellationToken);

            // checks if there are any records in the database from the filter
            if (!result.AnySafe())
            {
                // reply with standard message for record not found
                return NotFound(Messages.RECORD_NOT_FOUND
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<IList<Contact>>());
            }

            return Ok(result.ToBusiness());
        }

        /// <summary>
        /// Create contact for customer
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Contact>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Contact>>), StatusCodes.Status400BadRequest)]
        [Route("{customerId:int}/Contact", Name = "ContactCreate")]
        public async Task<ActionResult<IBusinessResult<IList<Contact>>>> Create(int customerId, [FromBody] Contact model, CancellationToken cancellationToken)
        {
            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            var errors = model.TryValidate(validator);

            if (!errors.AnySafe())
            {
                model.CustomerId = customerId;
                await Repository.AddAsync(model, cancellationToken);
                if (await unitOfWork.SaveChangesAsync(cancellationToken) > 0)
                {
                    return Created(nameof(Create), model.ToBusiness());
                }
            }
            // get message in request context, if not, use default message
            return BadRequest(errors
                .ToBusiness<Contact>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error))
            );
        }

        /// <summary>
        /// Update customer contact
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Contact>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Contact>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Contact>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [Route("{customerId:int}/Contact/{id}", Name = "ContactUpdate")]
        public async Task<ActionResult<IBusinessResult<IList<Contact>>>> Update(int customerId, int id, [FromBody] Contact model, CancellationToken cancellationToken)
        {
            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            var errors = model.TryValidate(validator);

            if (!errors.AnySafe())
            {
                // get entity through contact and customer identifier
                var modelDb = await Repository.GetByAsync(x => x.Id == id && x.CustomerId == customerId, cancellationToken)
                    .FirstOrDefaultAsync();
                if (modelDb == null)
                {
                    return NotFound(Messages.RECORD_NOT_FOUND_FOR_ID
                        .ToMessageResult(MessageType.Error)
                            .ToBusiness<Contact>());
                }

                // properties that cannot be overridden
                model.Id = modelDb.Id;
                model.CustomerId = modelDb.CustomerId;
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
            return BadRequest(errors
                .ToBusiness<Contact>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error))
            );
        }

        /// <summary>
        /// Delete customer contact
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Contact>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Contact>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<Contact>>), StatusCodes.Status400BadRequest)]
        [Route("{customerId:int}/Contact/{id}", Name = "ContactDelete")]
        public async Task<ActionResult<IBusinessResult<IList<Contact>>>> Delete(int customerId, int id, CancellationToken cancellationToken)
        {
            // try to retrieve entity by identifier
            var model = await Repository.GetByAsync(x => x.Id == id && x.CustomerId == customerId, cancellationToken)
                .FirstOrDefaultAsync();
            if (model == null)
            {
                return NotFound(Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<Contact>());
            }

            // perform delete action
            await Repository.RemoveAsync(model, cancellationToken);
            if (await unitOfWork.SaveChangesAsync(cancellationToken) > 0)
            {
                return Ok(Messages.OPERATION_SUCCESS
                    .ToMessageResult(MessageType.Success)
                        .ToBusiness<Contact>());
            }
            // get message in request context, if not, use default message
            return BadRequest(Messages.OPERATION_FAIL
                .ToMessageResult(MessageType.Error)
                .ToBusiness<Contact>()
            );
        }

        #endregion
    }
}
