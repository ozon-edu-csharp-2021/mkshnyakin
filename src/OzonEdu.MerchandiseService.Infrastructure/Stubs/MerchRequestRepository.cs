﻿using System.Collections.Concurrent;
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
                var employee1 = EmployeeId.Create(1);
                {
                    var merchRequest = new MerchRequest(employee1, RequestMerchType.WelcomePack, CreationMode.System);
                    merchRequest.Complete(Date.Create("11/23/2021 13:14:00"));
                    Create(merchRequest);
                }
                {
                    var merchRequest =
                        new MerchRequest(employee1, RequestMerchType.ConferenceListenerPack, CreationMode.User);
                    merchRequest.Complete(Date.Create("10/15/2021 08:05:01"));
                    Create(merchRequest);
                }
            }

            {
                var employee2 = EmployeeId.Create(2);
                {
                    var merchRequest =
                        new MerchRequest(employee2, RequestMerchType.ConferenceListenerPack, CreationMode.User);
                    merchRequest.Complete(Date.Create("10/15/2021 08:05:01"));
                    Create(merchRequest);
                }
                {
                    var merchRequest =
                        new MerchRequest(employee2, RequestMerchType.ConferenceSpeakerPack, CreationMode.User);
                    merchRequest.SetStatus(ProcessStatus.OutOfStock);
                    Create(merchRequest);
                }
            }
            
            // Мерч для сотрудника выдавался больше года назад
            {
                var employee3 = EmployeeId.Create(3);
                {
                    var merchRequest = new MerchRequest(employee3, RequestMerchType.WelcomePack, CreationMode.User);
                    merchRequest.Complete(Date.Create("10/15/2019 08:05:01"));
                    Create(merchRequest);
                }
            }
            
            // Мерч для сотрудника выдавался меньше года назад
            {
                var employee4 = EmployeeId.Create(4);
                {
                    var merchRequest = new MerchRequest(employee4, RequestMerchType.WelcomePack, CreationMode.User);
                    merchRequest.Complete(Date.Create("11/11/2021 08:05:01"));
                    Create(merchRequest);
                }
            }         
            
            // Мерч для сотрудника в OutOfStock
            {
                var employee5 = EmployeeId.Create(5);
                {
                    var merchRequest = new MerchRequest(employee5, RequestMerchType.WelcomePack, CreationMode.User);                    
                    merchRequest.SetStatus(ProcessStatus.OutOfStock);
                    Create(merchRequest);
                }
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

        public Task<IEnumerable<MerchRequest>> FindByEmployeeIdAsync(
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            var result = Items.Values.Where(x => x.EmployeeId.Value == employeeId);
            return Task.FromResult(result);
        }
        
        public Task<IEnumerable<MerchRequest>> FindByRequestMerchTypeAsync(
            RequestMerchType requestMerchType,
            CancellationToken cancellationToken = default)
        {
            var result = Items.Values.Where(x => x.MerchType.Equals(requestMerchType));
            return Task.FromResult(result);
        }

        public Task<IEnumerable<MerchRequest>> FindCompletedByEmployeeIdAsync(
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            var result = Items.Values.Where(x =>
                x.EmployeeId.Value.Equals(employeeId)
                && x.Status.Equals(ProcessStatus.Complete));
            return Task.FromResult(result);
        }
    }
}