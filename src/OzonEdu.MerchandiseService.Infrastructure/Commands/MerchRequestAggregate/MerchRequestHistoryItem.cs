using System;

namespace OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate
{
    public class MerchRequestHistoryItem
    {
        public MerchPackHistoryItem Item { get; init; }
        public DateTime GiveOutDate { get; init; }
    }
}