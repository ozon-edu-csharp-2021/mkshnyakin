using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchPackItemAggregate
{
    public class MerchPackItemTests
    {
        public static object[][] InvalidParams => new[]
        {
            new object[] {null, null},

            new object[] {ItemName.Create("Ololo"), null},
            new object[] {null, Sku.Create(1)}
        };

        [Fact]
        public void MerchPackItemCreation_ReturnCorrectEntity_WhenParamsIsValid()
        {
            var merchPackItem = new MerchPackItem(ItemName.Create("Ololo"), Sku.Create(1));
            Assert.IsType<MerchPackItem>(merchPackItem);
        }

        [Theory]
        [MemberData(nameof(InvalidParams))]
        public void MerchPackItemCreation_ThrowsCorruptedInvariantException_WhenParamsIsNull(ItemName itemName, Sku sku)
        {
            Assert.Throws<CorruptedInvariantException>(() => new MerchPackItem(itemName, sku));
        }
    }
}