using CustomerAPI.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Mvp24Hours.Core.Contract.Data;

namespace CustomerAPI.Infrastructure.Builders
{
    public class CustomerConfiguration : IBsonClassMap<Customer>
    {
        public void Configure()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Customer)))
            {
                BsonClassMap.RegisterClassMap<Customer>(cm =>
                {
                    cm.AutoMap();
                });
            }
        }
    }
}
