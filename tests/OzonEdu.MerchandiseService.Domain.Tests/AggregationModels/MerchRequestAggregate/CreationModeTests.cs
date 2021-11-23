using System.Collections.Generic;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchRequestAggregate
{
    public class CreationModeTests
    {
        public static IEnumerable<object[]> CreationModeParams => Enumeration.GetAll<CreationMode>()
            .Select(x => new object[] {x});

        public static IEnumerable<object[]> InvalidCreationModeParams => new CreationMode[]
            {
                new(int.MinValue, nameof(int.MinValue)),
                new(int.MaxValue, nameof(int.MaxValue))
            }
            .Select(x => new object[] {x});

        [Theory]
        [MemberData(nameof(CreationModeParams))]
        public void GetById_ReturnsCorrectValues_WhenIdIsValid(CreationMode creationMode)
        {
            var instance = Enumeration.GetById<CreationMode>(creationMode.Id);
            Assert.Equal(creationMode, instance);
        }

        [Theory]
        [MemberData(nameof(InvalidCreationModeParams))]
        public void GetById_ThrowsCorruptedValueObjectException_WhenIdIsInvalid(CreationMode creationMode)
        {
            Assert.Throws<CorruptedValueObjectException>(() => Enumeration.GetById<CreationMode>(creationMode.Id));
        }
    }
}