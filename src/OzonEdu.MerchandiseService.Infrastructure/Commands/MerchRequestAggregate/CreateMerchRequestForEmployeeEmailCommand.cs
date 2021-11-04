using CSharpCourse.Core.Lib.Enums;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate
{
    public class CreateMerchRequestForEmployeeEmailCommand : IRequest<CreateMerchRequestResponse>
    {
        public Email EmployeeEmail { get; init; }
        public MerchType MerchType { get; init; }
    }
}