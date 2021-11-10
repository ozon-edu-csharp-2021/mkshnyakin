using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;

namespace OzonEdu.MerchandiseService.Domain.DomainServices
{
    public sealed class MerchRequestService
    {
        /// <summary>
        /// Создание запроса на выдачу мерча от пользователся
        /// </summary>
        /// <param name="employee">Сотрудник</param>
        /// <param name="requestMerchType">Тип запрашиваемого мерча</param>
        /// <param name="employeeMerchRequests">Все имеющиеся запросы сотрудника на выдачу мерча</param>
        /// <param name="currentDate">Текущая дата</param>
        /// <returns>Новый запрос или null при ошибке</returns>
        public static MerchRequest ProcessUserMerchRequest(
            Employee employee,
            RequestMerchType requestMerchType,
            IEnumerable<MerchRequest> employeeMerchRequests,
            Date currentDate)
        {
            var firstIncomplete = employeeMerchRequests.FirstOrDefault(x =>
                x.MerchType.Equals(requestMerchType)
                && !x.Status.Equals(ProcessStatus.Complete));

            if (firstIncomplete != default)
            {
                return firstIncomplete;
            }

            var yearAgo = currentDate.Value - TimeSpan.FromDays(365);
            var firstLessThanYear = employeeMerchRequests.FirstOrDefault(x =>
                x.MerchType.Equals(requestMerchType)
                && x.Status.Equals(ProcessStatus.Complete)
                && x.GiveOutDate.Value > yearAgo);

            if (firstLessThanYear != default)
            {
                return firstLessThanYear;
            }

            var result = new MerchRequest(EmployeeId.Create(employee.Id), requestMerchType, CreationMode.User);
            return result;
        }
    }
}