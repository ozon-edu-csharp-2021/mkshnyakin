using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Models;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate
{
    public class CreateMerchRequestCommandHandler : IRequestHandler<CreateMerchRequestCommand, int>
    {
        private readonly IMerchRequestRepository _merchRequestRepository;

        public CreateMerchRequestCommandHandler(IMerchRequestRepository merchRequestRepository)
        {
            _merchRequestRepository = merchRequestRepository;
        }

        public async Task<int> Handle(CreateMerchRequestCommand request, CancellationToken cancellationToken)
        {
            /*
            var stockInDb = await _stockItemRepository.FindBySkuAsync(new Sku(request.Sku), cancellationToken);
            if (stockInDb is not null)
                throw new Exception($"Stock item with sku {request.Sku} already exist");

            var newStockItem = new StockItem(
                new Sku(request.Sku),
                new Name(request.Name),
                new Item(ItemType
                    .GetAll<ItemType>()
                    .FirstOrDefault(it => it.Id.Equals(request.StockItemType))),
                Enumeration
                    .GetAll<ClothingSize>()
                    .FirstOrDefault(it => it.Id.Equals(request.ClothingSize)),
                new Quantity(request.Quantity),
                new QuantityValue(request.MinimalQuantity)
            );

            var createResult = await _stockItemRepository.CreateAsync(newStockItem, cancellationToken);
            await _stockItemRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            
            return newStockItem.Id;
            */
            return await Task.FromResult(111);
        }
    }
}