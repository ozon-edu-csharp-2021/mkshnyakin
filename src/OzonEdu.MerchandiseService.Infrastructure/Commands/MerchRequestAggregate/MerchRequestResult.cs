using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate
{
    public class MerchRequestResult
    {
        public bool IsSuccess { get; private init; }
        public ProcessStatus Status { get; private init; }
        public string Message { get; private init; }
        public long? RequestId { get; private init; }

        public static MerchRequestResult Fail(string errorMessage)
            => new MerchRequestResult
            {
                IsSuccess = false,
                Message = errorMessage,
            };

        public static MerchRequestResult Success(ProcessStatus status, string message, long requestId)
            => new MerchRequestResult
            {
                IsSuccess = true,
                Status = status,
                Message = message,
                RequestId = requestId,
            };
    }
}