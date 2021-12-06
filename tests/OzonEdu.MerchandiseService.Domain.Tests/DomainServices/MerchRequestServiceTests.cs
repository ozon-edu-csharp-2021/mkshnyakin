using System;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.DomainServices;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.DomainServices
{
    public class MerchRequestServiceTests
    {
        private static readonly Employee Employee1 = new(
            1,
            PersonName.Create("John", "Michael", "Doe"),
            Email.Create("ololo@example.com"));

        [Fact]
        public void ProcessUserMerchRequest_ReturnNewDraftMerchRequest_WhenHistoryIsEmpty()
        {
            var employee = Employee1;
            var merchType = RequestMerchType.WelcomePack;
            var mode = CreationMode.User;
            var employeeMerchRequests = Enumerable.Empty<MerchRequest>();
            var date = Date.Create(DateTime.MaxValue);

            var merchRequest = MerchRequestService.ProcessUserMerchRequest(
                employee,
                merchType,
                mode,
                employeeMerchRequests,
                date);

            Assert.IsType<MerchRequest>(merchRequest);
            Assert.Equal(ProcessStatus.Draft, merchRequest.Status);
            Assert.Equal(merchType, merchRequest.MerchType);
            Assert.Equal(mode, merchRequest.Mode);
            Assert.Equal(employee.Id, merchRequest.EmployeeId.Value);
        }

        [Fact]
        public void ProcessUserMerchRequest_ReturnFirstIncompleteMerchRequest_WhenIncompleteIsPresent()
        {
            var employee = Employee1;
            var merchType = RequestMerchType.WelcomePack;
            var mode = CreationMode.User;
            var employeeId = EmployeeId.Create(employee.Id);
            var now = Date.Create(DateTime.Now);
            var merchRequest1 = new MerchRequest(1, employeeId, merchType, ProcessStatus.Complete, mode, now, false);
            var merchRequest2 = new MerchRequest(2, employeeId, merchType, ProcessStatus.OutOfStock, mode, null, false);
            var employeeMerchRequests = new[] {merchRequest1, merchRequest2};
            var date = Date.Create(DateTime.MaxValue);

            var merchRequest = MerchRequestService.ProcessUserMerchRequest(
                employee,
                merchType,
                mode,
                employeeMerchRequests,
                date);

            Assert.Equal(merchRequest2, merchRequest);
        }

        [Fact]
        public void ProcessUserMerchRequest_ReturnFirstCompletedMerchRequest_WhenCompletedLessThanYearAgo()
        {
            var employee = Employee1;
            var merchType = RequestMerchType.WelcomePack;
            var mode = CreationMode.User;
            var employeeId = EmployeeId.Create(employee.Id);
            var minDate = Date.Create(DateTime.MinValue);
            var month11Ago = Date.Create(DateTime.Now.AddMonths(-11));
            var merchRequest1 = new MerchRequest(1, employeeId, merchType, ProcessStatus.Complete, mode, minDate, false);
            var merchRequest2 = new MerchRequest(2, employeeId, merchType, ProcessStatus.Complete, mode, month11Ago, false);
            var employeeMerchRequests = new[] {merchRequest1, merchRequest2};
            var date = Date.Create(DateTime.Now);

            var merchRequest = MerchRequestService.ProcessUserMerchRequest(
                employee,
                merchType,
                mode,
                employeeMerchRequests,
                date);

            Assert.Equal(merchRequest2, merchRequest);
        }

        [Fact]
        public void ProcessUserMerchRequest_ReturnNewDraftMerchRequest_WhenCompletedMoreThanYearAgo()
        {
            var employee = Employee1;
            var merchType = RequestMerchType.WelcomePack;
            var mode = CreationMode.User;
            var employeeId = EmployeeId.Create(employee.Id);
            var minDate = Date.Create(DateTime.MinValue);
            var month13Ago = Date.Create(DateTime.Now.AddMonths(-13));
            var merchRequest1 = new MerchRequest(1, employeeId, merchType, ProcessStatus.Complete, mode, minDate, false);
            var merchRequest2 = new MerchRequest(2, employeeId, merchType, ProcessStatus.Complete, mode, month13Ago, false);
            var employeeMerchRequests = new[] {merchRequest1, merchRequest2};
            var date = Date.Create(DateTime.Now);

            var merchRequest = MerchRequestService.ProcessUserMerchRequest(
                employee,
                merchType,
                mode,
                employeeMerchRequests,
                date);

            Assert.IsType<MerchRequest>(merchRequest);
            Assert.Equal(ProcessStatus.Draft, merchRequest.Status);
            Assert.Equal(merchType, merchRequest.MerchType);
            Assert.Equal(mode, merchRequest.Mode);
            Assert.Equal(employee.Id, merchRequest.EmployeeId.Value);
        }
    }
}