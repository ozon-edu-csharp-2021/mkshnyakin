using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public sealed class MerchRequest : Entity
    {
        public ProcessStatus Status { get; private set; }
        public CreationMode Mode { get; private set; }
        public RequestMerchType RequestMerchType { get; private set; }
        
    }
}