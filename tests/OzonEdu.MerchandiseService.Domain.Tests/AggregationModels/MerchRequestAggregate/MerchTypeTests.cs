using System;
using System.Collections.Generic;
using System.Linq;
using CSharpCourse.Core.Lib.Enums;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Models;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchRequestAggregate
{
    public class MerchTypeTests
    {
        public static IEnumerable<object[]> MerchTypeValues => Enum.GetValues<MerchType>()
            .Select(x => new object[] {x});

        public static IEnumerable<object[]> RequestMerchTypeValues => Enumeration.GetAll<RequestMerchType>()
            .Select(x => new object[] {x});

        [Theory]
        [MemberData(nameof(MerchTypeValues))]
        public void RequestMerchType_Contains_AllMerchTypes(MerchType merchType)
        {
            var merchTypeInt = (int) merchType;
            var requestMerchTypeValues = Enumeration.GetAll<RequestMerchType>();
            Assert.Contains(requestMerchTypeValues, x => x.Id == merchTypeInt);
        }

        [Theory]
        [MemberData(nameof(RequestMerchTypeValues))]
        public void MerchType_Contains_AllRequestMerchTypes(RequestMerchType requestMerchType)
        {
            var merchType = (MerchType) requestMerchType.Id;
            var merchTypeValues = Enum.GetValues<MerchType>();
            Assert.Contains(merchType, merchTypeValues);
        }
    }
}