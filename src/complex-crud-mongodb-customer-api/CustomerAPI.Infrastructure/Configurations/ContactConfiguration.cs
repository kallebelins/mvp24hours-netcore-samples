using CustomerAPI.Core.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Entities;

namespace CustomerAPI.Infrastructure.Builders
{
    public class ContactConfiguration : IBsonClassMap<Contact>
    {
        public void Configure()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Contact)))
            {
                BsonClassMap.RegisterClassMap<EntityBase<Contact, string>>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdProperty(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
                    cm.IdMemberMap.SetSerializer(new StringSerializer().WithRepresentation(BsonType.ObjectId));
                    cm.UnmapMember(x => x.EntityKey);
                });

                BsonClassMap.RegisterClassMap<Contact>(cm =>
                {
                    cm.AutoMap();
                });
            }
        }
    }
}
