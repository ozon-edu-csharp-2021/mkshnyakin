using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.EmployeeAggregate
{
    public class EmployeeTests
    {
        public static object[][] InvalidParams => new[]
        {
            new object[] {null, null},

            new object[] {PersonName.Create("John", "Michael", "Doe"), null},
            new object[] {null, Email.Create("ololo@example.com")}
        };

        [Fact]
        public void EmployeeCreation_ReturnCorrectEntity_WhenParamsIsValid()
        {
            var employee = new Employee(PersonName.Create("John", "Michael", "Doe"), Email.Create("ololo@example.com"));
            Assert.IsType<Employee>(employee);
        }

        [Theory]
        [MemberData(nameof(InvalidParams))]
        public void EmployeeCreation_ThrowsCorruptedInvariantException_WhenParamsIsNull(
            PersonName personName,
            Email email)
        {
            Assert.Throws<CorruptedInvariantException>(() => new Employee(personName, email));
        }
    }
}