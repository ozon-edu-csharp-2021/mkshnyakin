using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationServices;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;
using static OzonEdu.MerchandiseService.Domain.DomainServices.MerchRequestService;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate
{
    public class ProcessMerchRequestCommandHandler
        : IRequestHandler<ProcessMerchRequestCommand, MerchRequestResult>
    {
        private readonly IApplicationService _applicationService;
        private readonly ITracer _tracer;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessMerchRequestCommandHandler> _logger;

        public ProcessMerchRequestCommandHandler(
            IApplicationService applicationService,
            ITracer tracer,
            IUnitOfWork unitOfWork,
            ILogger<ProcessMerchRequestCommandHandler> logger)
        {
            _applicationService = applicationService;
            _tracer = tracer;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<MerchRequestResult> Handle(
            ProcessMerchRequestCommand command,
            CancellationToken cancellationToken)
        {
            using var span = _tracer
                .BuildSpan($"{nameof(ProcessMerchRequestCommandHandler)}.{nameof(Handle)}")
                .StartActive();

            var requestMerchType = command.MerchType.ToRequestMerchType();
            var creationMode = command.IsSystem ? CreationMode.System : CreationMode.User;
            var employeeEmail = command.EmployeeEmail;
            var employeeId = command.EmployeeId;

            var employee = await _applicationService.GetEmployee(employeeId, employeeEmail, cancellationToken)
                           ?? throw new ItemNotFoundException(
                               $"Employee (id:{employeeId}, email: {employeeEmail}) is not found");

            var employeeMerchRequests =
                await _applicationService.GetEmployeeMerchRequests(employee.Id, cancellationToken);

            var request = ProcessUserMerchRequest(
                employee,
                requestMerchType,
                creationMode,
                employeeMerchRequests,
                Date.Create(DateTime.Now));

            // Проверяется что такой мерч еще не выдавался сотруднику
            if (request.Status.Equals(ProcessStatus.Complete))
            {
                return MerchRequestResult.Fail(request.Status.ToString(), request.Id);
            }


            await _unitOfWork.StartTransactionAsync(cancellationToken);
            try
            {
                // Проверяется наличие данного мерча на складе через запрос к stock-api
                // Если все проверки прошли - зарезервировать мерч в stock-api
                var isComplete = await _applicationService.CanCheckAndReserve(request, employee, cancellationToken);
                if (isComplete)
                {
                    // Отметить у себя в БД, что сотруднику выдан мерч
                    request.Complete(Date.Create(DateTime.Now));
                }
                else
                {
                    //Если мерча нет в наличии - необходимо запомнить, что такой сотрудник запрашивал такой мерч
                    request.SetStatus(ProcessStatus.OutOfStock);
                }

                var savedRequest = await _applicationService.SaveRequest(request, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var response = savedRequest.Status.Equals(ProcessStatus.Complete)
                    ? MerchRequestResult.Success(savedRequest.Status.ToString(), savedRequest.Id)
                    : MerchRequestResult.Fail(savedRequest.Status.ToString(), savedRequest.Id);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while process transaction. Message: {Message}", e.Message);
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}