using System.Collections.Generic;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchPackItemAggregate
{
    public class SkuTests
    {
        public static IEnumerable<object[]> ValidSkuIds => new[]
        {
            1,
            2,
            3,
            long.MaxValue
        }.Select(e => new object[] {e});

        public static IEnumerable<object[]> InvalidSkuIds => new[]
        {
            0,
            -1,
            long.MinValue
        }.Select(e => new object[] {e});

        [Theory]
        [MemberData(nameof(ValidSkuIds))]
        public void SkuCreation_ReturnCorrectValueObject_WhenSkuIdIsValid(long skuId)
        {
            var sku = Sku.Create(skuId);
            Assert.Equal(skuId, sku.Id);
        }

        [Theory]
        [MemberData(nameof(InvalidSkuIds))]
        public void SkuCreation_ThrowsCorruptedValueObjectException_WhenSkuIdIsInvalid(long skuId)
        {
            Assert.Throws<CorruptedValueObjectException>(() => Sku.Create(skuId));
        }
    }
}