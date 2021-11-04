using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate
{
    public sealed class MerchPackItem : Entity
    {
        public MerchPackItem(ItemName itemName, Sku sku)
        {
            ItemName = itemName;
            Sku = sku;
        }

        public ItemName ItemName { get; }
        public Sku Sku { get; }
    }
}