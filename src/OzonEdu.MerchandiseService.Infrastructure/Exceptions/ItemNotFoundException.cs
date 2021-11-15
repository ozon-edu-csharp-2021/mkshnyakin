using System;
using System.Runtime.Serialization;

namespace OzonEdu.MerchandiseService.Infrastructure.Exceptions
{
    [Serializable]
    public class ItemNotFoundException : InfrastructureException
    {
        public ItemNotFoundException()
        {
        }

        protected ItemNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ItemNotFoundException(string message) : base(message)
        {
        }

        public ItemNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}