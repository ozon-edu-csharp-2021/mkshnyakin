using System;
using System.Runtime.Serialization;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;

namespace OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.Exceptions
{
    public class NoActiveTransactionStartedException : InfrastructureException
    {
        public NoActiveTransactionStartedException()
        {
        }

        protected NoActiveTransactionStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NoActiveTransactionStartedException(string message) : base(message)
        {
        }

        public NoActiveTransactionStartedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}