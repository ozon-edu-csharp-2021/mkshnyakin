using System.Collections.Generic;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchRequestAggregate
{
    public class ProcessStatusTests
    {
        public static IEnumerable<object[]> ProcessStatusParams => Enumeration.GetAll<ProcessStatus>()
            .Select(x => new object[] {x});

        public static IEnumerable<object[]> InvalidProcessStatusParams => new ProcessStatus[]
            {
                new(int.MinValue, nameof(int.MinValue)),
                new(int.MaxValue, nameof(int.MaxValue))
            }
            .Select(x => new object[] {x});

        [Theory]
        [MemberData(nameof(ProcessStatusParams))]
        public void GetById_ReturnsCorrectValues_WhenIdIsValid(ProcessStatus processStatus)
        {
            var instance = Enumeration.GetById<ProcessStatus>(processStatus.Id);
            Assert.Equal(processStatus, instance);
        }

        [Theory]
        [MemberData(nameof(InvalidProcessStatusParams))]
        public void GetById_ThrowsCorruptedValueObjectException_WhenIdIsInvalid(ProcessStatus processStatus)
        {
            Assert.Throws<CorruptedValueObjectException>(() => Enumeration.GetById<ProcessStatus>(processStatus.Id));
        }
    }
}