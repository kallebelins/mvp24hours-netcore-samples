using CustomerAPI.Core.Enums;
using Mvp24Hours.Core.Entities;
using System.Text.Json.Serialization;

namespace CustomerAPI.Core.Entities
{
    public class Contact : EntityBaseLog<int, string>
    {
        [JsonIgnore]
        public int CustomerId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContactType Type { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public Customer Customer { get; set; }
    }
}
