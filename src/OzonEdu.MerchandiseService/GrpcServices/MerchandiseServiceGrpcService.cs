using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpCourse.Core.Lib.Enums;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using OpenTracing;
using OzonEdu.MerchandiseService.Grpc;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;

namespace OzonEdu.MerchandiseService.GrpcServices
{
    public class MerchandiseServiceGrpcService : MerchandiseServiceGrpc.MerchandiseServiceGrpcBase
    {
        private readonly IMediator _mediator;
        private readonly ITracer _tracer;

        public MerchandiseServiceGrpcService(IMediator mediator, ITracer tracer)
        {
            _mediator = mediator;
            _tracer = tracer;
        }

        public override async Task<GetHistoryForEmployeeResponse> GetHistoryForEmployee(
            EmployeeMerchHistoryRequest request,
            ServerCallContext context)
        {
            using var span = _tracer
                .BuildSpan($"{nameof(MerchandiseServiceGrpcService)}.{nameof(GetHistoryForEmployee)}")
                .StartActive();
            span.Span.SetTag("protocol", "gRPC");
            span.Span.SetTag(nameof(request.EmployeeId), request.EmployeeId);
            
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
            using var span = _tracer
                .BuildSpan($"{nameof(MerchandiseServiceGrpcService)}.{nameof(RequestMerchForEmployee)}")
                .StartActive();
            span.Span.SetTag("protocol", "gRPC");
            span.Span.SetTag(nameof(request.EmployeeId), request.EmployeeId);
            span.Span.SetTag(nameof(request.MerchType), ((MerchType) request.MerchType).ToString());
            
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
                span.Span.SetTag(nameof(result.IsSuccess), result.IsSuccess);
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