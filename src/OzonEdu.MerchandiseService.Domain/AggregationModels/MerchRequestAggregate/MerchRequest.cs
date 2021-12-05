using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public sealed class MerchRequest : Entity
    {
        public MerchRequest(EmployeeId employeeId, RequestMerchType merchType, CreationMode mode)
        {
            EmployeeId = employeeId ?? throw new CorruptedInvariantException($"{nameof(employeeId)} is null");
            MerchType = merchType ?? throw new CorruptedInvariantException($"{nameof(merchType)} is null");
            Mode = mode ?? throw new CorruptedInvariantException($"{nameof(mode)} is null");
            Status = ProcessStatus.Draft;
        }

        public MerchRequest(
            long id,
            EmployeeId employeeId,
            RequestMerchType merchType,
            ProcessStatus status,
            CreationMode mode,
            Date giveOutDate,
            bool isEmailSended
            )
        {
            EmployeeId = employeeId;
            MerchType = merchType;
            Status = status;
            Mode = mode;
            GiveOutDate = giveOutDate;
            Id = id;
            IsEmailSended = isEmailSended;
        }

        public EmployeeId EmployeeId { get; }
        public RequestMerchType MerchType { get; }
        public ProcessStatus Status { get; private set; }
        public CreationMode Mode { get; }
        public Date GiveOutDate { get; private set; }
        public bool IsEmailSended { get; private set; }

        public void SetStatus(ProcessStatus status)
        {
            if (status is null) throw new CorruptedInvariantException($"{nameof(status)} is null");

            Status = !Status.Equals(ProcessStatus.Complete) && !status.Equals(ProcessStatus.Complete)
                ? status
                : throw new CorruptedInvariantException($"Incorrect status: {status}");

            if (Status.Equals(ProcessStatus.OutOfStock))
            {
                IsEmailSended = false;
            }
        }

        public void Complete(Date date)
        {
            Status = !Status.Equals(ProcessStatus.Complete)
                ? ProcessStatus.Complete
                : throw new CorruptedInvariantException($"Current status is incorrect: {Status}");
            GiveOutDate = date ?? throw new CorruptedInvariantException($"{nameof(date)} is null");
        }

        public void SendEmail()
        {
            IsEmailSended = true;
        }

        public override string ToString()
        {
            return $"Id: {Id}. {EmployeeId}. {Status}. {Mode}. {MerchType}. {GiveOutDate}";
        }
    }
}