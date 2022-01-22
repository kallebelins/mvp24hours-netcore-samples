using CustomerAPI.Application.Pipe.Operations.Customers;
using CustomerAPI.Core.Contract.Pipe.Builders;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;

namespace CustomerAPI.Application.Pipe.Builders
{
    public class GetByIdCustomerBuilder : IGetByIdCustomerBuilder
    {
        public IPipelineAsync Builder(IPipelineAsync pipeline) => pipeline
            .Add<GetCustomerClientStep>()
            .Add<GetByIdCustomerMapperResponseStep>();
    }
}
