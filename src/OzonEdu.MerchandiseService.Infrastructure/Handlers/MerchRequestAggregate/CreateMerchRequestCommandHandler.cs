using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate
{
    public class CreateMerchRequestCommandHandler : IRequestHandler<CreateMerchRequestForEmployeeIdCommand, CreateMerchRequestResponse>
    {
        private readonly IOzonEduEmployeeServiceClient _employeeClient;
        private readonly IMerchRequestRepository _merchRequestRepository;

        public CreateMerchRequestCommandHandler(IMerchRequestRepository merchRequestRepository,
            IOzonEduEmployeeServiceClient employeeClient)
        {
            _merchRequestRepository = merchRequestRepository;
            _employeeClient = employeeClient;
        }

        public async Task<CreateMerchRequestResponse> Handle(CreateMerchRequestForEmployeeIdCommand requestForEmployeeId,
            CancellationToken cancellationToken)
        {
            var employeeId = requestForEmployeeId.EmployeeId;
            var employeeViewModel = await _employeeClient.GetByIdAsync(employeeId)
                                    ?? throw new EmployeeNotFoundException($"Employee (id:{employeeId}) is not found");

            var employee = new Employee(
                employeeViewModel.Id,
                PersonName.Create(
                    employeeViewModel.FirstName,
                    employeeViewModel.MiddleName,
                    employeeViewModel.LastName),
                Email.Parse(employeeViewModel.Email)
            );

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
            var response = new CreateMerchRequestResponse{};
            return await Task.FromResult(response);

        }
    }

    public class
        CreateMerchRequestForEmployeeEmailCommandHandler : IRequestHandler<CreateMerchRequestForEmployeeEmailCommand,
            CreateMerchRequestResponse>
    {
        private readonly IOzonEduEmployeeServiceClient _employeeClient;

        public CreateMerchRequestForEmployeeEmailCommandHandler(IOzonEduEmployeeServiceClient employeeClient)
        {
            _employeeClient = employeeClient;
        }

        public async Task<CreateMerchRequestResponse> Handle(CreateMerchRequestForEmployeeEmailCommand requestForEmployeeId,
            CancellationToken cancellationToken)
        {
            var employeeEmail = requestForEmployeeId.EmployeeEmail;
            var employeeViewModel = await _employeeClient.FindByEmailAsync(employeeEmail.Value)
                                    ?? throw new EmployeeNotFoundException(
                                        $"Employee (email:{employeeEmail}) is not found");

            
            
            var employee = new Employee(
                employeeViewModel.Id,
                PersonName.Create(
                    employeeViewModel.FirstName,
                    employeeViewModel.MiddleName,
                    employeeViewModel.LastName),
                Email.Parse(employeeViewModel.Email)
            );

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
            var response = new CreateMerchRequestResponse{};
            return await Task.FromResult(response);
        }
    }
}