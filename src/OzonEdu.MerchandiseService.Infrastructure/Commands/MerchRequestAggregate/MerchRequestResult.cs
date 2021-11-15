using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate
{
    public class MerchRequestResult
    {
        public bool IsSuccess { get; private init; }
        public string Message { get; private init; }
        public long RequestId { get; private init; }

        public static MerchRequestResult Fail(string errorMessage, long requestId = 0)
            => new MerchRequestResult
            {
                IsSuccess = false,
                Message = errorMessage,
                RequestId = requestId,
            };

        public static MerchRequestResult Success(string message, long requestId)
            => new MerchRequestResult
            {
                IsSuccess = true,
                Message = message,
                RequestId = requestId,
            };
    }
}