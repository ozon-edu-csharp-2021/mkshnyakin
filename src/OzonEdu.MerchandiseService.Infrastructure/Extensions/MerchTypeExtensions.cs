using System;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Models;
using CSharpCourse.Core.Lib.Enums;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class MerchTypeExtensions
    {
        public static RequestMerchType ToRequestMerchType(this MerchType merchType)
        {
            var domainMerchType = Enumeration.GetAll<RequestMerchType>().FirstOrDefault(t => t.Id == (int) merchType);
            return domainMerchType ?? throw Exception();
        }

        public static MerchType ToMerchType(this RequestMerchType requestMerchType)
        {
            var merchType = (MerchType) requestMerchType.Id;
            return Enum.GetValues<MerchType>().Contains(merchType)
                ? merchType
                : throw Exception();
        }
    }
}