using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using OpenTracing;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationServices;
using OzonEdu.MerchandiseService.Infrastructure.Cache;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Exceptions;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;
using static OzonEdu.MerchandiseService.Domain.DomainServices.MerchRequestService;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate
{
    public class ProcessMerchRequestCommandHandler
        : IRequestHandler<ProcessMerchRequestCommand, MerchRequestResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMerchRequestRepository _merchRequestRepository;
        private readonly IApplicationService _applicationService;
        private readonly IDistributedCache _cache;
        private readonly CacheKeysProvider _cacheKeys;
        private readonly ITracer _tracer;
        
        public ProcessMerchRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IMerchRequestRepository merchRequestRepository,
            IApplicationService applicationService,
            IDistributedCache cache,
            CacheKeysProvider cacheKeys,
            ITracer tracer)
        {
            _unitOfWork = unitOfWork;
            _merchRequestRepository = merchRequestRepository;
            _applicationService = applicationService;
            _cache = cache;
            _cacheKeys = cacheKeys;
            _tracer = tracer;
        }

        public async Task<MerchRequestResult> Handle(
            ProcessMerchRequestCommand command,
            CancellationToken cancellationToken)
        {
            using var span = _tracer
                .BuildSpan(nameof(ProcessMerchRequestCommandHandler))
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


            // Проверяется наличие данного мерча на складе через запрос к stock-api
            // Если все проверки прошли - зарезервировать мерч в stock-api
            var isComplete = await _applicationService.CheckAndReserve(request, employee, cancellationToken);
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

            var savedRequest = await SaveRequest(request, cancellationToken);

            var response = savedRequest.Status.Equals(ProcessStatus.Complete)
                ? MerchRequestResult.Success(savedRequest.Status.ToString(), savedRequest.Id)
                : MerchRequestResult.Fail(savedRequest.Status.ToString(), savedRequest.Id);

            return response;
        }

        private async Task<MerchRequest> SaveRequest(MerchRequest merchRequest, CancellationToken cancellationToken)
        {
            using var span = _tracer.BuildSpan(nameof(SaveRequest)).StartActive();
            
            var key = _cacheKeys.GetMerchRequestHistoryKey(merchRequest.EmployeeId.Value);
            await _cache.RemoveAsync(key, cancellationToken);

            await _unitOfWork.StartTransactionAsync(cancellationToken);
            try
            {
                if (merchRequest.Id == 0)
                {
                    merchRequest = await _merchRequestRepository.CreateAsync(merchRequest, cancellationToken);
                }
                else
                {
                    merchRequest = await _merchRequestRepository.UpdateAsync(merchRequest, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
            
            return merchRequest;
        }
    }
}