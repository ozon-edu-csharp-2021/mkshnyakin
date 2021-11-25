using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.Contracts.MessageBus;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;
using OzonEdu.MerchandiseService.Infrastructure.Stubs;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationServices
{
    public interface IApplicationService
    {
        Task<Employee> GetEmployee(long id, string email, CancellationToken cancellationToken = default);

        Task<IEnumerable<MerchRequest>> GetEmployeeMerchRequests(
            long id,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<MerchPackItem>> GetMerchPackItemsByType(
            RequestMerchType requestMerchType,
            CancellationToken cancellationToken = default);

        Task<bool> CheckAndReserve(
            MerchRequest request,
            Employee employee,
            CancellationToken cancellationToken = default);
    }

    public sealed class ApplicationService : IApplicationService
    {
        private readonly IOzonEduEmployeeServiceClient _employeeClient;
        private readonly IMerchPackItemRepository _merchPackItemRepository;
        private readonly IMerchRequestRepository _merchRequestRepository;
        private readonly IOzonEduStockApiClient _ozonEduStockApiClient;
        private readonly IMessageBus _messageBus;

        public ApplicationService(
            IOzonEduEmployeeServiceClient employeeClient,
            IMerchPackItemRepository merchPackItemRepository,
            IMerchRequestRepository merchRequestRepository,
            IOzonEduStockApiClient ozonEduStockApiClient,
            IMessageBus messageBus)
        {
            _employeeClient = employeeClient;
            _merchPackItemRepository = merchPackItemRepository;
            _merchRequestRepository = merchRequestRepository;
            _ozonEduStockApiClient = ozonEduStockApiClient;
            _messageBus = messageBus;
        }

        public async Task<Employee> GetEmployee(long id, string email, CancellationToken cancellationToken = default)
        {
            OzonEduEmployeeServiceClient.EmployeeViewModel employeeViewModel;
            try
            {
                if (!string.IsNullOrWhiteSpace(email))
                    employeeViewModel = await _employeeClient.GetByEmailAsync(email, cancellationToken);
                else
                    employeeViewModel = await _employeeClient.GetByIdAsync(id, cancellationToken);
            }
            catch (Exception e)
            {
                throw new ItemNotFoundException(
                    $"Employee (id:{id}, email: {email}) not found because service crashed",
                    e);
            }

            if (employeeViewModel is null)
            {
                throw new ItemNotFoundException(
                    $"Employee (id:{id}, email: {email}) not found in OzonEduEmployeeService");
            }

            var employee = employeeViewModel.ToEmployee();
            return employee;
        }

        public async Task<IEnumerable<MerchRequest>> GetEmployeeMerchRequests(
            long id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var employeeMerchRequests = await _merchRequestRepository.FindByEmployeeIdAsync(id, cancellationToken);
                return employeeMerchRequests;
            }
            catch (Exception e)
            {
                throw new ItemNotFoundException(
                    $"Employee (id:{id}) merch requests not found because service crashed",
                    e);
            }
        }

        public async Task<IEnumerable<MerchPackItem>> GetMerchPackItemsByType(
            RequestMerchType requestMerchType,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var merchPackItems =
                    await _merchPackItemRepository.FindByMerchTypeAsync(requestMerchType, cancellationToken);
                return merchPackItems;
            }
            catch (Exception e)
            {
                throw new ItemNotFoundException(
                    $"MerchPackItems not found for type {requestMerchType} because service crashed",
                    e);
            }
        }

        public async Task<bool> CheckAndReserve(
            MerchRequest request,
            Employee employee,
            CancellationToken cancellationToken = default)
        {
            var isItemsAvailable = false;
            var isItemsReserved = false;
            var merchPackItems = await GetMerchPackItemsByType(request.MerchType, cancellationToken);
            var skus = merchPackItems.Select(x => x.Sku.Id).ToArray();
            var items = skus.ToArray();

            try
            {
                isItemsAvailable = await _ozonEduStockApiClient.IsAvailable(items, cancellationToken);
            }
            catch (Exception e)
            {
                throw new ItemNotFoundException("MerchPackItems not available because service crashed", e);
            }

            try
            {
                isItemsReserved = await _ozonEduStockApiClient.Reserve(items, cancellationToken);
            }
            catch (Exception e)
            {
                throw new ItemNotFoundException("MerchPackItems not reserved because service crashed", e);
            }

            var isAllOk = isItemsAvailable && isItemsReserved;

            if (request.Mode.Equals(CreationMode.System))
            {
                if (isAllOk)
                {
                    var employeeEmailMessage = new EmailMessage
                    {
                        ToEmail = employee.Email.Value,
                        ToName = employee.Name.ToString(),
                        Subject = "Необходимо подойти к HR для получения мерча",
                        Body = string.Empty
                    };
                    _messageBus.Notify(employeeEmailMessage);
                }
                else
                {
                    var employeeEmailMessage = new EmailMessage
                    {
                        ToEmail = "hr@ozon.example.com",
                        ToName = "HR department",
                        Subject = "Мерч закончился и необходимо сделать поставку",
                        Body = $"Список SkuId: {string.Join(", ", items)}"
                    };
                    _messageBus.Notify(employeeEmailMessage);
                }
            }

            return isAllOk;
        }
    }
}