using System;
using System.Collections.Generic;
using System.Linq;
using CSharpCourse.Core.Lib.Enums;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Models;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchRequestAggregate
{
    public class RequestMerchTypeTests
    {
        private static readonly IEnumerable<RequestMerchType> RequestMerchTypeValues =
            Enumeration.GetAll<RequestMerchType>();

        private static readonly RequestMerchType[] InvalidRequestMerchTypeValues =
        {
            new(int.MinValue, nameof(int.MinValue)),
            new(int.MaxValue, nameof(int.MaxValue))
        };

        private static readonly MerchType[] MerchTypeValues = Enum.GetValues<MerchType>();

        private static readonly MerchType[] InvalidMerchTypeValues =
        {
            (MerchType) int.MinValue,
            (MerchType) int.MaxValue
        };

        public static IEnumerable<object[]> MerchTypeParams => MerchTypeValues
            .Select(x => new object[] {x});

        public static IEnumerable<object[]> InvalidMerchTypeParams => InvalidMerchTypeValues
            .Select(x => new object[] {x});

        public static IEnumerable<object[]> RequestMerchTypeParams => RequestMerchTypeValues
            .Select(x => new object[] {x});

        public static IEnumerable<object[]> InvalidRequestMerchTypeParams => InvalidRequestMerchTypeValues
            .Select(x => new object[] {x});

        [Theory]
        [MemberData(nameof(MerchTypeParams))]
        public void RequestMerchType_Contains_AllMerchTypeValues(MerchType merchType)
        {
            var merchTypeInt = (int) merchType;
            Assert.Contains(RequestMerchTypeValues, x => x.Id == merchTypeInt);
        }

        [Theory]
        [MemberData(nameof(RequestMerchTypeParams))]
        public void MerchType_Contains_AllRequestMerchTypeValues(RequestMerchType requestMerchType)
        {
            var merchType = (MerchType) requestMerchType.Id;
            Assert.Contains(merchType, MerchTypeValues);
        }

        [Theory]
        [MemberData(nameof(MerchTypeParams))]
        public void ToRequestMerchTypeConversion_ReturnCorrectValue_ForMerchTypeValues(MerchType merchType)
        {
            var requestMerchType = merchType.ToRequestMerchType();
            Assert.Contains(requestMerchType, RequestMerchTypeValues);
        }

        [Theory]
        [MemberData(nameof(RequestMerchTypeParams))]
        public void ToMerchTypeConversion_ReturnCorrectValue_ForRequestMerchTypeValues(
            RequestMerchType requestMerchType)
        {
            var merchType = requestMerchType.ToMerchType();
            Assert.Contains(merchType, MerchTypeValues);
        }

        [Theory]
        [MemberData(nameof(InvalidMerchTypeParams))]
        public void ToRequestMerchTypeConversion_ThrowsItemNotFoundException_WhenMerchTypeIsInvalid(MerchType merchType)
        {
            Assert.Throws<ItemNotFoundException>(() => merchType.ToRequestMerchType());
        }

        [Theory]
        [MemberData(nameof(InvalidRequestMerchTypeParams))]
        public void ToMerchTypeConversion_ThrowsItemNotFoundException_WhenRequestMerchTypeIsInvalid(
            RequestMerchType requestMerchType)
        {
            Assert.Throws<ItemNotFoundException>(() => requestMerchType.ToMerchType());
        }
    }
}