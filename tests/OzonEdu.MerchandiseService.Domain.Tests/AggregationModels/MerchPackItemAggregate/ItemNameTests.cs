using System.Collections.Generic;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchPackItemAggregate
{
    public class ItemNameTests
    {
        public static IEnumerable<object[]> ValidParams => new[]
        {
            "1",
            "ololo",
            "27\" Монитор Samsung S27R650FDI, серый"
        }.Select(e => new object[] {e});

        public static IEnumerable<object[]> InvalidParams => new[]
        {
            null,
            string.Empty,
            "",
            " ",
            "    ",
            "\r",
            "\r\r\r\r\r",
            "\n",
            "\n\n\n\n\n\n\n",
            "\t",
            "\t\t\t\t\t",
            "\r\n",
            "\r\n\r\n\r\n\r\n\r\n\r\n"
        }.Select(e => new object[] {e});


        [Theory]
        [MemberData(nameof(ValidParams))]
        public void ItemNameCreation_ReturnCorrectValueObject_WhenParamIsValid(string itemNameString)
        {
            var itemName = ItemName.Create(itemNameString);
            Assert.Equal(itemNameString, itemName.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidParams))]
        public void ItemNameCreation_ThrowsCorruptedValueObjectException_WhenParamIsInvalid(string itemNameString)
        {
            Assert.Throws<CorruptedValueObjectException>(() => ItemName.Create(itemNameString));
        }
    }
}