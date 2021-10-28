using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using OzonEdu.MerchandiseService.Grpc;

namespace OzonEdu.MerchandiseService.GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var cts = new CancellationTokenSource();
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new MerchandiseServiceGrpc.MerchandiseServiceGrpcClient(channel);

            try
            {
                var response = await client.GetHistoryForEmployeeAsync(
                    new EmployeeMerchRequest {EmployeeId = 1},
                    cancellationToken: cts.Token);
                foreach (var item in response.Items)
                {
                    Console.WriteLine(
                        $"Name: {item.Item.Name}. Sku: {item.Item.SkuId}. Date: {item.Date.ToDateTime()}.");
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine();

            try
            {
                var response = await client.RequestMerchForEmployeeAsync(
                    new EmployeeMerchRequest {EmployeeId = 5},
                    cancellationToken: cts.Token);
                var items = response?.Items ?? Enumerable.Empty<EmployeeMerchItem>();
                foreach (var item in items)
                {
                    Console.WriteLine($"Name: {item.Name}. Sku: {item.SkuId}.");
                }
            }
            catch (RpcException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}