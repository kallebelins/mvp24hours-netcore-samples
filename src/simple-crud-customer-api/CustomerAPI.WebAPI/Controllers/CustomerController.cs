using CustomerAPI.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Core.Extensions;
using Mvp24Hours.Infrastructure.Extensions;
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
        private readonly IUnitOfWorkAsync unitOfWork;

        private IRepositoryAsync<Customer> repository
        {
            get
            {
                return unitOfWork.GetRepositoryAsync<Customer>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CustomerController(IUnitOfWorkAsync uoW)
        {
            unitOfWork = uoW;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IPagingResult<IList<Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult<IPagingResult<IList<Customer>>>> GetBy([FromQuery] Customer filter, [FromQuery] PagingCriteriaRequest clause, CancellationToken cancellationToken)
        {
            var models = await repository.GetByAsync(x => 
                !filter.Name.HasValue() || x.Name.Contains(filter.Name), 
                clause.ToPagingService(), 
                cancellationToken
            );
            return models.AnyOrNotNull() ? Ok(models) : NotFound();
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IBusinessResult<Customer>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}", Name = "CustomerGetById")]
        public async Task<ActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var model = await repository.GetByIdAsync(id, cancellationToken);
            return model != null ? Ok(model) : NotFound();
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IBusinessResult<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("", Name = "CustomerCreate")]
        public async Task<ActionResult> Create([FromBody] Customer model, CancellationToken cancellationToken)
        {
            await repository.AddAsync(model, cancellationToken);

            if (await unitOfWork.SaveChangesAsync(cancellationToken) > 0)
            {
                Created(nameof(Create), model);
            }
            return BadRequest();
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id}", Name = "CustomerUpdate")]
        public async Task<ActionResult> Update(int id, [FromBody] Customer model, CancellationToken cancellationToken)
        {
            model.Id = id;

            await repository.ModifyAsync(model, cancellationToken);

            if (await unitOfWork.SaveChangesAsync(cancellationToken) > 0)
            {
                Created(nameof(Update), model);
            }
            return BadRequest();
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id}", Name = "CustomerDelete")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var model = await repository.GetByIdAsync(id, cancellationToken);

            if (model == null)
            {
                return NotFound();
            }

            await repository.RemoveAsync(model, cancellationToken);

            if (await unitOfWork.SaveChangesAsync(cancellationToken) > 0)
            {
                Ok();
            }
            return BadRequest();
        }

    }
}
