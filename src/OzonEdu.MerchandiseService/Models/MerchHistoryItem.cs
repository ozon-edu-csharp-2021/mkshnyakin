using System;

namespace OzonEdu.MerchandiseService.Models
{
    public class MerchHistoryItem
    {
        public MerchItem Item { get; init; }
        public DateTime Date { get; init; }
    }
}