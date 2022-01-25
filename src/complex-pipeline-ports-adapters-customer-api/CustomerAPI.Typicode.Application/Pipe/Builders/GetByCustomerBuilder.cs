using CustomerAPI.Core.Contract.Pipe.Builders;
using CustomerAPI.Typicode.Application.Pipe.Operations.Customers;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;

namespace CustomerAPI.Typicode.Application.Pipe.Builders
{
    public class GetByCustomerBuilder : IGetByCustomerBuilder
    {
        public IPipelineAsync Builder(IPipelineAsync pipeline) => pipeline
            .Add<GetCustomerClientStep>()
            .Add<GetByCustomerMapperResponseStep>();
    }
}
