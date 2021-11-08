using System.Collections.Generic;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.EmployeeAggregate
{
    public class PersonNameTests
    {
        public static IEnumerable<object[]> ValidNames => new[]
        {
            new object[] {"John", string.Empty, "Doe"},
            new object[] {"John", "Michael", "Doe"},
            new object[] {"Jane", "Emily", "Doe"},
            new object[] {"Jane", string.Empty, "Doe"},
            new object[] {"Антон", "Фёдорович", "Глушков"}
        };

        public static IEnumerable<object[]> InvalidNames => new[]
        {
            new object[] {null, null, null},

            new object[] {string.Empty, null, null},
            new object[] {null, string.Empty, null},
            new object[] {null, null, string.Empty},

            new object[] {string.Empty, string.Empty, null},
            new object[] {string.Empty, null, string.Empty},
            new object[] {null, string.Empty, string.Empty},

            new object[] {string.Empty, string.Empty, string.Empty},

            new object[] {"John", null, null},
            new object[] {"John", string.Empty, null},
            new object[] {"John", null, string.Empty},
            new object[] {"John", null, "Doe"},
            new object[] {null, null, "Doe"},
            new object[] {null, string.Empty, "Doe"},
            new object[] {string.Empty, string.Empty, "Doe"},
            new object[] {string.Empty, null, "Doe"}
        };

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void PersonNameCreation_ReturnCorrectValueObject_WhenParamsIsValid(
            string firstName,
            string middleName,
            string lastName)
        {
            var personName = PersonName.Create(firstName, middleName, lastName);
            Assert.Equal(firstName, personName.FirstName);
            Assert.Equal(middleName, personName.MiddleName);
            Assert.Equal(lastName, personName.LastName);
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void PersonNameCreation_ThrowsCorruptedValueObjectException_WhenParamsIsInvalid(
            string firstName,
            string middleName,
            string lastName)
        {
            Assert.Throws<CorruptedValueObjectException>(() => PersonName.Create(firstName, middleName, lastName));
        }
    }
}