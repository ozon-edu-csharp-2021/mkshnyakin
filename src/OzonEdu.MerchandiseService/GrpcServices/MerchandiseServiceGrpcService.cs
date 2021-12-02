using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpCourse.Core.Lib.Enums;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using OzonEdu.MerchandiseService.Grpc;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;

namespace OzonEdu.MerchandiseService.GrpcServices
{
    public class MerchandiseServiceGrpcService : MerchandiseServiceGrpc.MerchandiseServiceGrpcBase
    {
        private readonly IMediator _mediator;

        public MerchandiseServiceGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<GetHistoryForEmployeeResponse> GetHistoryForEmployee(
            EmployeeMerchHistoryRequest request,
            ServerCallContext context)
        {
            IEnumerable<MerchRequestHistoryItem> history = null;
            var getMerchRequestHistoryForEmployeeIdCommand = new GetMerchRequestHistoryForEmployeeIdCommand
            {
                EmployeeId = request.EmployeeId
            };
            try
            {
                history = await _mediator.Send(getMerchRequestHistoryForEmployeeIdCommand, context.CancellationToken);
            }
            catch (ItemNotFoundException e)
            {
                var status = new Status(StatusCode.NotFound, e.Message);
                var metadata = new Metadata
                {
                    {nameof(request.EmployeeId), request.EmployeeId.ToString()}
                };
                throw new RpcException(status, metadata);
            }
            catch (Exception e)
            {
                throw CreateInternalRpcException(e);
            }

            var items = history ?? Enumerable.Empty<MerchRequestHistoryItem>();
            var convertedItems = items.Select(x => new EmployeeMerchGetResponseItem
            {
                Item = new EmployeeMerchItem
                {
                    Name = x.Item.Name,
                    SkuId = x.Item.Sku
                },
                Date = Timestamp.FromDateTime(DateTime.SpecifyKind(x.GiveOutDate, DateTimeKind.Utc))
            });

            var result = new GetHistoryForEmployeeResponse
            {
                Items = {convertedItems}
            };

            return result;
        }

        public override async Task<RequestMerchForEmployeeResponse> RequestMerchForEmployee(
            EmployeeMerchRequest request,
            ServerCallContext context)
        {
            var processUserMerchRequestCommand = new ProcessMerchRequestCommand
            {
                EmployeeId = request.EmployeeId,
                MerchType = (MerchType) request.MerchType,
                IsSystem = false
            };

            try
            {
                var result = await _mediator.Send(processUserMerchRequestCommand, context.CancellationToken);
                var response = new RequestMerchForEmployeeResponse
                {
                    IsSuccess = result.IsSuccess,
                    RequestId = result.RequestId,
                    Message = result.Message
                };
                return response;
            }
            catch (ItemNotFoundException e)
            {
                var status = new Status(StatusCode.NotFound, e.Message);
                var metadata = new Metadata
                {
                    {nameof(request.EmployeeId), request.EmployeeId.ToString()},
                    {nameof(request.MerchType), request.MerchType.ToString()}
                };
                throw new RpcException(status, metadata);
            }
            catch (Exception e)
            {
                throw CreateInternalRpcException(e);
            }
        }

        private static RpcException CreateInternalRpcException(Exception e)
        {
            var status = new Status(StatusCode.Internal, e.Message);
            var metadata = new Metadata
            {
                {"ExceptionType", e.GetType().FullName}
                //{"StackTrace", e.StackTrace}, // BUG: Если раскомментировать, то будет StatusCode.Unknown с пустыми Trailers
            };
            var rpcException = new RpcException(status, metadata);
            return rpcException;
        }
    }
}