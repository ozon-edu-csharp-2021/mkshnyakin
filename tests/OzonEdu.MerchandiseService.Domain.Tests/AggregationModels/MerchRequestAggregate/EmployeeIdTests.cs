using System.Collections.Generic;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchRequestAggregate
{
    public class EmployeeIdTests
    {
        public static IEnumerable<object[]> ValidIds => new[]
        {
            1,
            2,
            3,
            long.MaxValue
        }.Select(e => new object[] {e});

        public static IEnumerable<object[]> InvalidIds => new[]
        {
            0,
            -1,
            long.MinValue
        }.Select(e => new object[] {e});

        [Theory]
        [MemberData(nameof(ValidIds))]
        public void EmployeeIdCreation_ReturnCorrectValueObject_WhenEmployeeIdIsValid(long id)
        {
            var employeeId = EmployeeId.Create(id);
            Assert.Equal(id, employeeId.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidIds))]
        public void EmployeeIdCreation_ThrowsCorruptedValueObjectException_WhenEmployeeIdIsInvalid(long id)
        {
            Assert.Throws<CorruptedValueObjectException>(() => EmployeeId.Create(id));
        }
    }
}