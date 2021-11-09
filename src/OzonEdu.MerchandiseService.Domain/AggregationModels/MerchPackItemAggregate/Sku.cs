using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate
{
    public sealed class Sku : ValueObject
    {
        private Sku(long id)
        {
            Id = id;
        }

        public long Id { get; }

        public static Sku Create(long id)
        {
            return IsValid(id)
                ? new Sku(id)
                : throw new CorruptedValueObjectException($"{nameof(Sku)} is invalid. Id: {id}");
        }

        public override string ToString()
        {
            return $"SkuId: {Id}";
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
        }

        private static bool IsValid(long id)
        {
            return id > 0;
        }
    }
}