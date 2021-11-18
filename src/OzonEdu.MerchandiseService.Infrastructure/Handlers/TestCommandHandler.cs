using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Commands;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers
{
    public class TestCommandHandler : IRequestHandler<TestCommand>
    {
        private readonly IMerchRequestRepository _merchRequestRepository;
        private readonly IMerchPackItemRepository _merchPackItemRepository;

        public TestCommandHandler(
            IMerchRequestRepository merchRequestRepository,
            IMerchPackItemRepository merchPackItemRepository)
        {
            _merchRequestRepository = merchRequestRepository;
            _merchPackItemRepository = merchPackItemRepository;
        }

        public async Task<Unit> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            //var result = await _merchRequestRepository.GetByIdAsync(3, cancellationToken);

            /*
            var merchRequest = new MerchRequest(EmployeeId.Create(1), RequestMerchType.WelcomePack, CreationMode.System);
            merchRequest.Complete(Date.Create("11/23/2021 23:23:23"));
            var result = await _merchRequestRepository.CreateAsync(merchRequest, cancellationToken);
            */

            /*
            var merchRequest = await _merchRequestRepository.GetByIdAsync(8, cancellationToken);
            //merchRequest.Complete(Date.Create("02/21/2021 21:21:21"));
            merchRequest = await _merchRequestRepository.UpdateAsync(merchRequest, cancellationToken);
            */

            /*
            var merchRequest = await _merchRequestRepository.GetByIdAsync(9, cancellationToken);
            merchRequest = await _merchRequestRepository.DeleteAsync(merchRequest, cancellationToken);
            */

            //var items = await _merchRequestRepository.FindByEmployeeIdAsync(1, cancellationToken);


            //var items = await _merchRequestRepository.FindCompletedByEmployeeIdAsync(5, cancellationToken);

            //var items = await _merchRequestRepository.FindOutOfStockByRequestMerchTypesAsync(new[] { RequestMerchType.WelcomePack, RequestMerchType.ConferenceSpeakerPack, RequestMerchType.VeteranPack, }, cancellationToken);

            /*
            var pen = new MerchPackItem(ItemName.Create("Ололо с логотипом Ozon"), Sku.Create(111));
            var item = await _merchPackItemRepository.CreateAsync(pen, cancellationToken);
            */

            /*
            var item = await _merchPackItemRepository.GetByIdAsync(8, cancellationToken);
            //item = await _merchPackItemRepository.UpdateAsync(item, cancellationToken);            
            item = await _merchPackItemRepository.DeleteAsync(item, cancellationToken);
            */

            /*
            var items = await _merchPackItemRepository.FindByMerchTypeAsync(RequestMerchType.WelcomePack, cancellationToken);
            items = await _merchPackItemRepository.FindByMerchTypeAsync(RequestMerchType.ConferenceListenerPack, cancellationToken);
            items = await _merchPackItemRepository.FindByMerchTypeAsync(RequestMerchType.ConferenceSpeakerPack, cancellationToken);
            items = await _merchPackItemRepository.FindByMerchTypeAsync(RequestMerchType.ProbationPeriodEndingPack, cancellationToken);
            items = await _merchPackItemRepository.FindByMerchTypeAsync(RequestMerchType.VeteranPack, cancellationToken);
            */

            /*
            var items = await _merchPackItemRepository.FindMerchTypesBySkuAsync(new[] {6L, 3}, cancellationToken);
            items = await _merchPackItemRepository.FindMerchTypesBySkuAsync(new[] {6L, 3, 1}, cancellationToken);
            */
            
            /*
            var item1 = await _merchPackItemRepository.GetByIdAsync(3, cancellationToken);
            var item2 = await _merchPackItemRepository.GetByIdAsync(4, cancellationToken);
            await _merchPackItemRepository.AddToPackAsync(RequestMerchType.VeteranPack, new[] {item1, item2}, cancellationToken);
            */

            return Unit.Value;
        }
    }
}