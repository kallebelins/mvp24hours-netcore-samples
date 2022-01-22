using Mvp24Hours.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace CustomerAPI.Core.ValueObjects
{
    public class MessageRequest : BaseVO
    {
        public MessageRequest()
        {
            Token = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public MessageRequest(Guid token, DateTime createDate)
        {
            Token = token;
            CreationDate = createDate;
        }

        public Guid Token { get; private set; }
        public DateTime CreationDate { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Token;
            yield return CreationDate;
        }
    }
}
