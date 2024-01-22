using CustomerAPI.Core.Entities;
using MongoDB.Bson.Serialization;
using Mvp24Hours.Core.Contract.Data;

namespace CustomerAPI.Infrastructure.Builders
{
    public class ContactConfiguration : IBsonClassMap<Contact>
    {
        public void Configure()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Contact)))
            {
                BsonClassMap.RegisterClassMap<Contact>(cm =>
                {
                    cm.AutoMap();
                });
            }
        }
    }
}
