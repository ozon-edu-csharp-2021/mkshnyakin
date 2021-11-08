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
        
        public MerchPackItem(long id, ItemName itemName, Sku sku) : this(itemName, sku)
        {
            Id = id;
        }

        public ItemName ItemName { get; }
        public Sku Sku { get; }
        
        public override string ToString()
        {
            return $"Id: {Id}. {ItemName}. {Sku}.";
        }
    }
}