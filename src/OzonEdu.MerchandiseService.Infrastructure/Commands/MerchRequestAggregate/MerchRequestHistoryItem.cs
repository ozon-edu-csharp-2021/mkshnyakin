using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate
{
    public class MerchRequestHistoryItem
    {
        public MerchPackItem Item { get; init; }
        public Date GiveOutDate { get; init; }
    }
}