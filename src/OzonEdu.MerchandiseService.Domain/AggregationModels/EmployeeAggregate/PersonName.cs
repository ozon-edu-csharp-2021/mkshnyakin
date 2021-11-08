using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate
{
    public sealed class PersonName : ValueObject
    {
        private PersonName(string firstName, string middleName, string lastName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string MiddleName { get; }
        public string LastName { get; }

        public static PersonName Create(string firstName, string middleName, string lastName)
        {
            return IsValid(firstName, middleName, lastName)
                ? new PersonName(firstName, middleName, lastName)
                : throw new CorruptedValueObjectException(
                    $"PersonName is invalid. FirstName: '{firstName}'. MiddleName: '{middleName}'. LastName: '{lastName}'");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName;
            yield return MiddleName;
            yield return LastName;
        }

        private static bool IsValid(string firstName, string middleName, string lastName)
        {
            return middleName is not null
                   && !string.IsNullOrEmpty(firstName)
                   && !string.IsNullOrEmpty(lastName);
        }

        public override string ToString()
        {
            return $"{FirstName} {MiddleName} {LastName}";
        }
    }
}