using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate
{
    public sealed class MerchPackItem : Entity
    {
        public MerchPackItem(ItemName itemName, Sku sku)
        {
            ItemName = itemName ?? throw new CorruptedInvariantException($"{nameof(itemName)} is null");
            Sku = sku ?? throw new CorruptedInvariantException($"{nameof(sku)} is null");
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