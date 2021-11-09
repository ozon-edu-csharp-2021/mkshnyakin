using System;
using System.Linq;
using CSharpCourse.Core.Lib.Enums;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Models;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class MerchTypeExtensions
    {
        public static RequestMerchType ToRequestMerchType(this MerchType merchType)
        {
            var merchTypeInt = (int) merchType;
            var domainMerchType = Enumeration.GetAll<RequestMerchType>().FirstOrDefault(t => t.Id == merchTypeInt);
            return domainMerchType ??
                   throw new ItemNotFoundException($"{nameof(RequestMerchType)} not found for {merchType}");
        }

        public static MerchType ToMerchType(this RequestMerchType requestMerchType)
        {
            var merchType = (MerchType) requestMerchType.Id;
            return Enum.GetValues<MerchType>().Contains(merchType)
                ? merchType
                : throw new ItemNotFoundException($"{nameof(MerchType)} not found for {requestMerchType}");
        }
    }
}