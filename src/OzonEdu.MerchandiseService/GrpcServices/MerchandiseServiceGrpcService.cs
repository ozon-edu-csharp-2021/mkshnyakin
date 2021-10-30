using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OzonEdu.MerchandiseService.Grpc;
using OzonEdu.MerchandiseService.Models;
using OzonEdu.MerchandiseService.Services;

namespace OzonEdu.MerchandiseService.GrpcServices
{
    public class MerchandiseServiceGrpcService : MerchandiseServiceGrpc.MerchandiseServiceGrpcBase
    {
        private readonly IMerchandiseService _merchandiseService;

        public MerchandiseServiceGrpcService(IMerchandiseService merchandiseService)
        {
            _merchandiseService = merchandiseService;
        }

        public override async Task<GetHistoryForEmployeeResponse> GetHistoryForEmployee(
            EmployeeMerchRequest request,
            ServerCallContext context)
        {
            IEnumerable<MerchHistoryItem> history = null;
            try
            {
                history = await _merchandiseService.GetHistoryForEmployee(
                    request.EmployeeId,
                    context.CancellationToken);
            }
            catch (Exception e)
            {
                throw CreateInternalRpcException(e);
            }

            var items = history ?? Enumerable.Empty<MerchHistoryItem>();
            var convertedItems = items.Select(x => new EmployeeMerchGetResponseItem
            {
                Item = new EmployeeMerchItem {Name = x.Item.Name, SkuId = x.Item.SkuId},
                Date = Timestamp.FromDateTime(DateTime.SpecifyKind(x.Date, DateTimeKind.Utc))
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
            var employeeId = request.EmployeeId;
            IEnumerable<MerchItem> items = null;
            try
            {
                items = await _merchandiseService.RequestMerchForEmployee(employeeId, context.CancellationToken);
            }
            catch (Exception e)
            {
                throw CreateInternalRpcException(e);
            }

            if (items is null)
            {
                var status = new Status(StatusCode.AlreadyExists, "Merch already given"); 
                var metadata = new Metadata
                {
                    {"employeeId", employeeId.ToString()},
                };
                throw new RpcException(status, metadata);
            }

            var response = new RequestMerchForEmployeeResponse
            {
                Items =
                {
                    items.Select(x => new EmployeeMerchItem
                    {
                        Name = x.Name,
                        SkuId = x.SkuId
                    })
                }
            };

            return response;
        }

        private static RpcException CreateInternalRpcException(Exception e)
        {
            var status = new Status(StatusCode.Internal, e.Message);
            var metadata = new Metadata
            {
                {"ExceptionType", e.GetType().FullName},
                //{"StackTrace", e.StackTrace}, // BUG: Если раскомментировать, то будет StatusCode.Unknown с пустыми Trailers
            };
            var rpcException = new RpcException(status, metadata);
            return rpcException;
        }
    }
}