using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public sealed class MerchRequest : Entity
    {
        
    }
    
    public enum MerchandizeRequestStatus
    {
        Draft,
        Created,
        Assigned,
        InProgress,
        Done,
    }
    
    //MerchRequest
}