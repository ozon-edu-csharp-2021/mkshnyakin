using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;
using static OzonEdu.MerchandiseService.Domain.DomainServices.MerchRequestService;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate
{
    public class ProcessUserMerchRequestCommandHandler
        : IRequestHandler<ProcessUserMerchRequestCommand, MerchRequestResult>
    {
        private readonly IOzonEduEmployeeServiceClient _employeeClient;
        private readonly IMerchPackItemRepository _merchPackItemRepository;
        private readonly IMerchRequestRepository _merchRequestRepository;

        public ProcessUserMerchRequestCommandHandler(
            IMerchRequestRepository merchRequestRepository,
            IOzonEduEmployeeServiceClient employeeClient,
            IMerchPackItemRepository merchPackItemRepository)
        {
            _merchRequestRepository = merchRequestRepository;
            _employeeClient = employeeClient;
            _merchPackItemRepository = merchPackItemRepository;
        }

        public async Task<MerchRequestResult> Handle(
            ProcessUserMerchRequestCommand requestForEmployeeId,
            CancellationToken cancellationToken)
        {
            var employeeId = requestForEmployeeId.EmployeeId;
            var employeeViewModel = await _employeeClient.GetByIdAsync(employeeId, cancellationToken)
                                    ?? throw new ItemNotFoundException($"Employee (id:{employeeId}) is not found");

            var employee = new Employee(
                employeeViewModel.Id,
                PersonName.Create(
                    employeeViewModel.FirstName,
                    employeeViewModel.MiddleName,
                    employeeViewModel.LastName),
                Email.Create(employeeViewModel.Email)
            );

            var requestMerchType = requestForEmployeeId.MerchType.ToRequestMerchType();

            var employeeMerchRequests =
                await _merchRequestRepository.FindByEmployeeIdAsync(employeeId, cancellationToken);

            var request = ProcessUserMerchRequest(
                employee,
                requestMerchType,
                employeeMerchRequests,
                Date.Create(DateTime.Now));

            // Проверяется что такой мерч еще не выдавался сотруднику
            if (request.Status.Equals(ProcessStatus.Complete))
            {
                return MerchRequestResult.Fail("ololo");
            }

            // Если Draft или OutOfStock, То проводим по сценарию 
            if (request.Status.Equals(ProcessStatus.Draft) || request.Status.Equals(ProcessStatus.OutOfStock))
            {
                // Проверяется наличие данного мерча на складе через запрос к stock-api
                // Если все проверки прошли - зарезервировать мерч в stock-api
                
                var merchItems =
                    await _merchPackItemRepository.FindByMerchTypeAsync(requestMerchType, cancellationToken);
                
                // Отметить у себя в БД, что сотруднику выдан мерч
                
                //Если мерча нет в наличии - необходимо запомнить, что такой сотрудник запрашивал такой мерч
            }


            var response = new MerchRequestResult();
            return await Task.FromResult(response);
        }
    }
}