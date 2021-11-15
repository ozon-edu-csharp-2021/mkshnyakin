using System;
using System.Runtime.Serialization;

namespace OzonEdu.MerchandiseService.Domain.Exceptions
{
    [Serializable]
    public class CorruptedInvariantException : DomainException
    {
        public CorruptedInvariantException()
        {
        }

        protected CorruptedInvariantException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CorruptedInvariantException(string message) : base(message)
        {
        }

        public CorruptedInvariantException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}