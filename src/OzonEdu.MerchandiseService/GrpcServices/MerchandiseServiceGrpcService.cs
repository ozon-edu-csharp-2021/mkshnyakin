using System;
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
            var history =
                await _merchandiseService.GetHistoryForEmployee(request.EmployeeId, context.CancellationToken);

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
            var items = await _merchandiseService.RequestMerchForEmployee(employeeId, context.CancellationToken);
            if (items is null)
            {
                throw new RpcException(
                    new Status(StatusCode.AlreadyExists, "Merch already given"),
                    new Metadata {new("employeeId", employeeId.ToString())});
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
    }
}