using System.Collections.Generic;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchRequestAggregate
{
    public class DateTests
    {
        public static IEnumerable<object[]> ValidDates => new[]
        {
            "2021/11/23",
            "1/1/2021",
            "11/23/2021",
            "2021-11-23",
            "11/23/2021 13:14:01",
            "2021-11-23 13:14:01",
            "05/01/2009 14:57:32.8",
            "2009-05-01 14:57:32.8",
            "2016-1-1 1:1:1",
            "2009-05-01T14:57:32.8375298-04:00",
            "5/01/2008",
            "5/01/2008 14:57:32.80 -07:00",
            "1 May 2008 2:57:32.8 PM",
            "Fri, 15 May 2009 20:10:57 GMT"
        }.Select(e => new object[] {e});

        public static IEnumerable<object[]> InvalidDates => new[]
        {
            null,
            string.Empty,
            "ololo",
            "-1",
            "1+2",

            "20211123",
            "2021/11/32",
            "2021/13/23",
            "2021/13/32",
            "16-05-2009 1:00:32 PM",

            "11/23/2021 13:60:01",
            "11/23/2021 24:14:01",
            "11/23/2021 13:00:60",
            "15/23/2021 13:14:01",
            "11/35/2021 13:14:01",

            "2021-11-23 13:64:01",
            "2021-11-23 25:14:01",
            "2021-11-23 23:14:61",
            "2021-15-23 13:14:01",
            "2021-11-35 13:14:01"
        }.Select(e => new object[] {e});

        [Theory]
        [MemberData(nameof(ValidDates))]
        public void DateCreation_ReturnCorrectValueObject_WhenDateIsValid(string dateString)
        {
            var date = Date.Create(dateString);
            Assert.NotNull(date);
        }

        [Theory]
        [MemberData(nameof(InvalidDates))]
        public void DateCreation_ThrowsCorruptedValueObjectException_WhenDateIsInvalid(string dateString)
        {
            Assert.Throws<CorruptedValueObjectException>(() => Date.Create(dateString));
        }
    }
}