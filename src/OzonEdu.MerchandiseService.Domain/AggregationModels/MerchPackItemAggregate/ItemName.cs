using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate
{
    public sealed class ItemName : ValueObject
    {
        private ItemName(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static ItemName Create(string itemName)
        {
            return IsValid(itemName)
                ? new ItemName(itemName)
                : throw new CorruptedValueObjectException($"ItemName is invalid: '{itemName}'");
        }

        public override string ToString()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private static bool IsValid(string itemName)
        {
            return !string.IsNullOrEmpty(itemName);
        }
    }
}