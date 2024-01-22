﻿using CustomerAPI.Core.Enums;
using Mvp24Hours.Core.Entities;
using System;
using System.Text.Json.Serialization;

namespace CustomerAPI.Core.Entities
{
    public class Contact : EntityBase<object>
    {
        public DateTime Created { get; set; }
        [JsonIgnore]
        public string CustomerId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContactType Type { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public Customer Customer { get; set; }
    }
}
