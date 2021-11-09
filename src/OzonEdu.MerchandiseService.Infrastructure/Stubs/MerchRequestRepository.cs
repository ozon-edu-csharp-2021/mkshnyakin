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
                var merchRequest =
                    new MerchRequest(EmployeeId.Create(1), RequestMerchType.WelcomePack, CreationMode.User);
                merchRequest.Complete(Date.Create("11/23/2021 13:14:00"));
                Create(merchRequest);
            }
            
            {
                var merchRequest =
                    new MerchRequest(EmployeeId.Create(2), RequestMerchType.ConferenceListenerPack, CreationMode.User);
                merchRequest.Complete(Date.Create("10/15/2021 08:05:01"));
                Create(merchRequest);
            }
            
            
            {
                var merchRequest =
                    new MerchRequest(EmployeeId.Create(2), RequestMerchType.ConferenceListenerPack, CreationMode.User);
                merchRequest.Complete(Date.Create("10/15/2021 08:05:01"));
                Create(merchRequest);
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
                merchRequest.GiveOutDate
            );
            var result = Items.TryAdd(id, newMerchRequest)
                ? newMerchRequest
                : null;
            return Task.FromResult(result);
        }

        public Task<MerchRequest> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<MerchRequest> UpdateAsync(MerchRequest itemToUpdate, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<MerchRequest> DeleteAsync(MerchRequest itemToUpdate, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<MerchRequest>> FindByRequestMerchTypeAsync(
            RequestMerchType requestMerchType,
            CancellationToken cancellationToken = default)
        {
            var result = Items.Values.Where(x => x.MerchType.Equals(requestMerchType));
            return Task.FromResult(result);
        }
    }
}