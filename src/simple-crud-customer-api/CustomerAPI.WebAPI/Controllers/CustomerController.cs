using CustomerAPI.Core.Entities;
using CustomerAPI.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs.Models;
using Mvp24Hours.Core.Extensions;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Infrastructure.Extensions;
using Mvp24Hours.WebAPI.Controller;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        /// 
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IPagingResult<IList<Customer>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("", Name = "CustomerGetBy")]
        public async Task<ActionResult> GetBy([FromQuery] CustomerFilter model, [FromQuery] PagingCriteriaRequest pagingCriteria, CancellationToken cancellationToken)
        {
            // filter
            Expression<Func<Customer, bool>> clause =
                x => (string.IsNullOrEmpty(model.Name) || x.Name.Contains(model.Name))
                    && (model.Active == null || x.Active == model.Active);
            // apply filter with pagination
            var result = await repository.ToBusinessPagingAsync(clause, pagingCriteria.ToPagingCriteria());
            return result.Summary.TotalCount > 0 ? Ok(result) : NotFound();
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
            var paging = new PagingCriteriaExpression<Customer>(3, 0);
            paging.NavigationExpr.Add(x => x.Contacts);
            var model = await repository.GetByIdAsync(id, paging, cancellationToken);
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
                return Created(nameof(Create), model);
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
                return Created(nameof(Update), model);
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
                return Ok();
            }
            return BadRequest();
        }

        #endregion
    }
}
