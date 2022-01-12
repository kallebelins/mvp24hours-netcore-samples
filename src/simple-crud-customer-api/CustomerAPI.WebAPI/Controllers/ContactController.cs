using CustomerAPI.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.WebAPI.Controller;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.WebAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/customer/[controller]")]
    [ApiController]
    public class ContactController : BaseMvpController
    {
        private readonly IUnitOfWorkAsync unitOfWork;

        private IRepositoryAsync<Contact> repository
        {
            get
            {
                return unitOfWork.GetRepositoryAsync<Contact>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ContactController(IUnitOfWorkAsync uoW)
        {
            unitOfWork = uoW;
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{customerId:int}/contact", Name = "ContactCreate")]
        public async Task<ActionResult> Create(int customerId, [FromBody] Contact model, CancellationToken cancellationToken)
        {
            model.CustomerId = customerId;

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
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{customerId:int}/contact/{id}", Name = "ContactUpdate")]
        public async Task<ActionResult> Update(int customerId, int id, [FromBody] Contact model, CancellationToken cancellationToken)
        {
            model.Id = id;
            model.CustomerId = customerId;

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
        [Route("{customerId:int}/contact/{id}", Name = "ContactDelete")]
        public async Task<ActionResult> Delete(int customerId, int id, CancellationToken cancellationToken)
        {
            var model = await repository.GetByAsync(x => x.Id == id && x.CustomerId == customerId, cancellationToken);

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
