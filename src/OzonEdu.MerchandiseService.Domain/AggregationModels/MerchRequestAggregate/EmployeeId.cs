using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public sealed class EmployeeId : ValueObject
    {
        private EmployeeId(long id)
        {
            Value = id;
        }

        public long Value { get; }

        public static EmployeeId Create(long id)
        {
            return IsValid(id)
                ? new EmployeeId(id)
                : throw new CorruptedValueObjectException($"{nameof(EmployeeId)} is invalid. Id: {id}");
        }

        public static EmployeeId Create(Employee employee)
        {
            return Create(employee.Id);
        }

        public override string ToString()
        {
            return $"EmployeeId: {Value}";
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private static bool IsValid(long id)
        {
            return id > 0;
        }
    }
}