﻿using CSharpCourse.Core.Lib.Enums;
using MediatR;

namespace OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate
{
    public class CreateMerchRequestForEmployeeIdCommand : IRequest<MerchRequestResult>
    {
        public long EmployeeId { get; init; }
        public MerchType MerchType { get; init; }
        public bool IsSystem { get; init; }
    }
}