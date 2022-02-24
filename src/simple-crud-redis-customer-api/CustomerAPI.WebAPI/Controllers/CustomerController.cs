using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Extensions;
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
        #region [ Fields / Properties ]

        private readonly IRepositoryCacheAsync<CustomerDto> _repositoryCache;
        private readonly IValidator<CustomerDto> validator;

        private IRepositoryCacheAsync<CustomerDto> RepositoryCache
        {
            get
            {
                return _repositoryCache;
            }
        }

        #endregion

        #region [ Ctors ]

        /// <summary>
        /// 
        /// </summary>
        public CustomerController(IRepositoryCacheAsync<CustomerDto> repositoryCache, IValidator<CustomerDto> validator)
        {
            _repositoryCache = repositoryCache;
            this.validator = validator;
        }

        #endregion

        #region [ Actions / Resources ]

        /// <summary>
        /// Get customer
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ActionResult<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult<CustomerDto>), StatusCodes.Status404NotFound)]
        [Route("{key}", Name = "CustomerGetById")]
        public async Task<ActionResult<CustomerDto>> GetById(string key, CancellationToken cancellationToken)
        {
            var result = await RepositoryCache.GetAsync(key, cancellationToken: cancellationToken);
            if (result == null)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Create customer
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ActionResult<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ActionResult<IBusinessResult<CustomerDto>>), StatusCodes.Status400BadRequest)]
        [Route("{key}", Name = "CustomerCreate")]
        public async Task<ActionResult> Create(string key, [FromBody] CustomerDto model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                return BadRequest(Messages.OPERATION_MODEL_REQUIRED
                            .ToMessageResult(MessageType.Error)
                                .ToBusiness<CustomerDto>());
            }

            var result = model.TryValidate(validator);
            if (result.AnySafe())
            {
                return BadRequest(result
                    .ToBusiness<CustomerDto>(
                        defaultMessage: Messages.OPERATION_FAIL
                            .ToMessageResult(MessageType.Error))
                );
            }

            await RepositoryCache.SetAsync(key, model, cancellationToken: cancellationToken);
            return Created(nameof(Create), key);
        }

        /// <summary>
        /// Delete customer
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("{key}", Name = "CustomerDelete")]
        public async Task<ActionResult> Delete(string key, CancellationToken cancellationToken)
        {
            await RepositoryCache.RemoveAsync(key, cancellationToken: cancellationToken);
            return Ok();
        }

        #endregion
    }
}
