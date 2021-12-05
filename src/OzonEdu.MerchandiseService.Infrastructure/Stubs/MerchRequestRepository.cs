using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public class MerchRequestRepository : IMerchRequestRepository
    {
        private static readonly IdentityGenerator IdGen = new();

        private static readonly ConcurrentDictionary<long, MerchRequest> Items = new();

        public MerchRequestRepository()
        {
            {
                var employee1 = EmployeeId.Create(1);
                {
                    var merchRequest = new MerchRequest(employee1, RequestMerchType.WelcomePack, CreationMode.System);
                    merchRequest.Complete(Date.Create("11/23/2021 13:14:00"));
                    Create(merchRequest);
                }
                {
                    var merchRequest =
                        new MerchRequest(employee1, RequestMerchType.ConferenceListenerPack, CreationMode.User);
                    merchRequest.Complete(Date.Create("10/15/2021 08:05:01"));
                    Create(merchRequest);
                }
            }

            {
                var employee2 = EmployeeId.Create(2);
                {
                    var merchRequest =
                        new MerchRequest(employee2, RequestMerchType.ConferenceListenerPack, CreationMode.User);
                    merchRequest.Complete(Date.Create("10/15/2021 08:05:01"));
                    Create(merchRequest);
                }
                {
                    var merchRequest =
                        new MerchRequest(employee2, RequestMerchType.ConferenceSpeakerPack, CreationMode.User);
                    merchRequest.SetStatus(ProcessStatus.OutOfStock);
                    Create(merchRequest);
                }
            }

            // Мерч для сотрудника выдавался больше года назад
            {
                var employee3 = EmployeeId.Create(3);
                {
                    var merchRequest = new MerchRequest(employee3, RequestMerchType.WelcomePack, CreationMode.User);
                    merchRequest.Complete(Date.Create("10/15/2019 08:05:01"));
                    Create(merchRequest);
                }
            }

            // Мерч для сотрудника выдавался меньше года назад
            {
                var employee4 = EmployeeId.Create(4);
                {
                    var merchRequest = new MerchRequest(employee4, RequestMerchType.WelcomePack, CreationMode.User);
                    merchRequest.Complete(Date.Create("11/11/2021 08:05:01"));
                    Create(merchRequest);
                }
            }

            // Мерч для сотрудника в OutOfStock
            {
                var employee5 = EmployeeId.Create(5);
                {
                    var merchRequest = new MerchRequest(employee5, RequestMerchType.WelcomePack, CreationMode.User);
                    merchRequest.SetStatus(ProcessStatus.OutOfStock);
                    Create(merchRequest);
                }
                {
                    var merchRequest = new MerchRequest(employee5, RequestMerchType.ConferenceListenerPack,
                        CreationMode.System);
                    merchRequest.SetStatus(ProcessStatus.OutOfStock);
                    Create(merchRequest);
                }
            }
        }


        private MerchRequest Create(MerchRequest merchRequest, CancellationToken cancellationToken = default)
        {
            return CreateAsync(merchRequest, cancellationToken).GetAwaiter().GetResult();
        }

        public Task<MerchRequest> CreateAsync(MerchRequest merchRequest, CancellationToken cancellationToken = default)
        {
            var id = IdGen.Get();
            var newMerchRequest = new MerchRequest(
                id,
                merchRequest.EmployeeId,
                merchRequest.MerchType,
                merchRequest.Status,
                merchRequest.Mode,
                merchRequest.GiveOutDate,
                merchRequest.IsEmailSended
            );
            var result = Items.TryAdd(id, newMerchRequest)
                ? newMerchRequest
                : null;
            return Task.FromResult(result);
        }

        public Task<MerchRequest> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            Items.TryGetValue(id, out var merchRequest);
            return Task.FromResult(merchRequest);
        }

        public Task<MerchRequest> UpdateAsync(MerchRequest itemToUpdate, CancellationToken cancellationToken = default)
        {
            Items[itemToUpdate.Id] = itemToUpdate;
            return Task.FromResult(Items[itemToUpdate.Id]);
        }

        public Task<MerchRequest> DeleteAsync(MerchRequest itemToDelete, CancellationToken cancellationToken = default)
        {
            Items.TryRemove(itemToDelete.Id, out var merchRequest);
            return Task.FromResult(merchRequest);
        }

        public Task<IEnumerable<MerchRequest>> FindByEmployeeIdAsync(
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            var result = Items.Values.Where(x => x.EmployeeId.Value == employeeId);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<MerchRequest>> FindCompletedByEmployeeIdAsync(
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            var result = Items.Values.Where(x =>
                x.EmployeeId.Value.Equals(employeeId)
                && x.Status.Equals(ProcessStatus.Complete));
            return Task.FromResult(result);
        }

        public Task<IEnumerable<MerchRequest>> FindOutOfStockByRequestMerchTypesAsync(
            IEnumerable<RequestMerchType> requestMerchTypes,
            CancellationToken cancellationToken = default)
        {
            var result = Items.Values.Where(x =>
                    x.Status.Equals(ProcessStatus.OutOfStock)
                    && requestMerchTypes.Contains(x.MerchType));
            return Task.FromResult(result);
        }
    }
}