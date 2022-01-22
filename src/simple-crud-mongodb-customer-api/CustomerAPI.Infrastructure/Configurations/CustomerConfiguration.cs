using CustomerAPI.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Entities;

namespace CustomerAPI.Infrastructure.Builders
{
    public class CustomerConfiguration : IBsonClassMap<Customer>
    {
        public void Configure()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Customer)))
            {
                BsonClassMap.RegisterClassMap<EntityBase<Customer, string>>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
                    cm.IdMemberMap.SetSerializer(new StringSerializer().WithRepresentation(BsonType.ObjectId));
                    cm.UnmapMember(x => x.EntityKey);
                });

                BsonClassMap.RegisterClassMap<Customer>(cm =>
                {
                    cm.AutoMap();
                });
            }
        }
    }
}
