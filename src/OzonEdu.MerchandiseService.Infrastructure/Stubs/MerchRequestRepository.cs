using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public class MerchRequestRepository : IMerchRequestRepository
    {
        public Task<MerchRequest> CreateAsync(MerchRequest itemToCreate, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
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
    }
}