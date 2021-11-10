using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.MerchRequestAggregate
{
    public class MerchRequestTests
    {
        public static object[][] InvalidParams => new[]
        {
            new object[] {null, null, null},

            new object[] {EmployeeId.Create(1), null, null},
            new object[] {null, RequestMerchType.WelcomePack, null},
            new object[] {null, null, CreationMode.User},

            new object[] {EmployeeId.Create(1), RequestMerchType.WelcomePack, null},
            new object[] {EmployeeId.Create(1), null, CreationMode.User},
            new object[] {null, RequestMerchType.WelcomePack, CreationMode.User}
        };

        private static MerchRequest CreateValidMerchRequest()
        {
            return new MerchRequest(EmployeeId.Create(1), RequestMerchType.WelcomePack, CreationMode.User);
        }

        [Fact]
        public void MerchRequestCreation_ReturnCorrectEntity_WhenParamsIsValid()
        {
            var merchRequest = CreateValidMerchRequest();
            Assert.IsType<MerchRequest>(merchRequest);
        }

        [Fact]
        public void MerchRequestCreation_HasCorrectStatus_WhenCreated()
        {
            var merchRequest = CreateValidMerchRequest();
            Assert.Equal(ProcessStatus.Draft, merchRequest.Status);
        }

        [Theory]
        [MemberData(nameof(InvalidParams))]
        public void MerchRequestCreation_ThrowsCorruptedInvariantException_WhenParamsIsNull(
            EmployeeId employeeId,
            RequestMerchType merchType,
            CreationMode mode)
        {
            Assert.Throws<CorruptedInvariantException>(() => new MerchRequest(employeeId, merchType, mode));
        }

        [Fact]
        public void MerchRequestSetStatus_SetCorrectStatus_WhenParamIsCorrect()
        {
            var merchRequest = CreateValidMerchRequest();
            var newStatus = ProcessStatus.OutOfStock;
            merchRequest.SetStatus(newStatus);
            Assert.Equal(newStatus, merchRequest.Status);
        }

        [Fact]
        public void MerchRequestSetStatus_ThrowsCorruptedInvariantException_WhenParamIsNull()
        {
            var merchRequest = CreateValidMerchRequest();
            Assert.Throws<CorruptedInvariantException>(() => merchRequest.SetStatus(null));
        }

        [Fact]
        public void MerchRequestSetStatus_ThrowsCorruptedInvariantException_WhenParamIsProcessStatusComplete()
        {
            var merchRequest = CreateValidMerchRequest();
            Assert.Throws<CorruptedInvariantException>(() => merchRequest.SetStatus(ProcessStatus.Complete));
        }

        [Fact]
        public void MerchRequestSetStatus_ThrowsCorruptedInvariantException_WhenCurrentStatusIsComplete()
        {
            var merchRequest = CreateValidMerchRequest();
            merchRequest.Complete(Date.Create("11/23/2021 13:14:00"));
            Assert.Throws<CorruptedInvariantException>(() => merchRequest.SetStatus(ProcessStatus.OutOfStock));
        }

        [Fact]
        public void MerchRequestComplete_SetCompleteStatus_WhenParamIsCorrect()
        {
            var merchRequest = CreateValidMerchRequest();
            merchRequest.Complete(Date.Create("11/23/2021 13:14:00"));
            Assert.Equal(ProcessStatus.Complete, merchRequest.Status);
        }

        [Fact]
        public void MerchRequestComplete_SetGiveOutDate_WhenParamIsCorrect()
        {
            var merchRequest = CreateValidMerchRequest();
            merchRequest.Complete(Date.Create("11/23/2021 13:14:00"));
            Assert.NotNull(merchRequest.GiveOutDate);
        }

        [Fact]
        public void MerchRequestComplete_ThrowsCorruptedInvariantException_WhenDateIsNull()
        {
            var merchRequest = CreateValidMerchRequest();
            Assert.Throws<CorruptedInvariantException>(() => merchRequest.Complete(null));
        }

        [Fact]
        public void MerchRequestComplete_ThrowsCorruptedInvariantException_WhenCurrentStatusIsComplete()
        {
            var merchRequest = CreateValidMerchRequest();
            merchRequest.Complete(Date.Create("11/23/2021 13:14:00"));
            Assert.Throws<CorruptedInvariantException>(() => merchRequest.Complete(Date.Create("11/23/2021")));
        }
    }
}