using System;
using System.Runtime.Serialization;

namespace OzonEdu.MerchandiseService.Domain.Exceptions
{
    [Serializable]
    public class CorruptedValueObjectException : DomainException
    {
        public CorruptedValueObjectException()
        {
        }

        protected CorruptedValueObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CorruptedValueObjectException(string message) : base(message)
        {
        }

        public CorruptedValueObjectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}