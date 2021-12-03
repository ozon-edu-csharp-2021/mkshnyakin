using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Enums;
using CSharpCourse.Core.Lib.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;
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
        private readonly EmailOptions _emailOptions;
        private readonly IProducer<string, string> _producer;
        private readonly KafkaConfiguration _kafkaOptions;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            IOzonEduEmployeeServiceClient employeeClient,
            IMerchPackItemRepository merchPackItemRepository,
            IMerchRequestRepository merchRequestRepository,
            IOzonEduStockApiClient ozonEduStockApiClient,
            IOptions<EmailOptions> emailOptions,
            IProducer<string, string> producer,
            IOptions<KafkaConfiguration> kafkaOptions)
        {
            _employeeClient = employeeClient;
            _merchPackItemRepository = merchPackItemRepository;
            _merchRequestRepository = merchRequestRepository;
            _ozonEduStockApiClient = ozonEduStockApiClient;
            _emailOptions = emailOptions.Value;
            _producer = producer;
            _kafkaOptions = kafkaOptions.Value;
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

            if (isItemsAvailable)
            {
                try
                {
                    isItemsReserved = await _ozonEduStockApiClient.Reserve(items, cancellationToken);
                }
                catch (Exception e)
                {
                    throw new ItemNotFoundException("MerchPackItems not reserved because service crashed", e);
                }
            }

            var isAllOk = isItemsAvailable && isItemsReserved;

            if (request.Mode.Equals(CreationMode.System))
            {
                try
                {
                    if (isAllOk)
                    {
                        var message = new Message<string, string>
                        {
                            Key = employee.Id.ToString(),
                            Value = JsonSerializer.Serialize(new NotificationEvent
                            {
                                EmployeeName = employee.Name.ToString(),
                                EmployeeEmail = employee.Email.Value,
                                EventType = EmployeeEventType.MerchDelivery,
                                Payload = new MerchDeliveryEventPayload
                                {
                                    MerchType = request.MerchType.ToMerchType(),
                                    ClothingSize = ClothingSize.L
                                }
                            })
                        };
                        _producer.Produce(_kafkaOptions.EmployeeNotificationEventTopic, message);
                    }
                    else
                    {
                        var message = new Message<string, string>
                        {
                            Key = employee.Id.ToString(),
                            Value = JsonSerializer.Serialize(new NotificationEvent
                            {
                                ManagerEmail = _emailOptions.HrToEmail,
                                EmployeeName = _emailOptions.HrToName,
                                EventType = EmployeeEventType.MerchDelivery,
                                Payload = new MerchDeliveryEventPayload
                                {
                                    MerchType = request.MerchType.ToMerchType(),
                                    ClothingSize = ClothingSize.L
                                }
                            })
                        };
                        _producer.Produce(_kafkaOptions.EmployeeNotificationEventTopic, message);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while sending event {Event}", nameof(NotificationEvent));
                }
            }

            return isAllOk;
        }
    }
}