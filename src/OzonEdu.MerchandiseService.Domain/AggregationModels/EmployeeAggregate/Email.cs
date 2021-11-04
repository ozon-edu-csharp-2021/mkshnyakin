using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate
{
    public sealed class Email : ValueObject
    {
        public Email(string value) => Value = value;

        public string Value { get; }

        public static Email Parse(string number)
        {
            // Do some parsing logic
            return new Email(number);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}