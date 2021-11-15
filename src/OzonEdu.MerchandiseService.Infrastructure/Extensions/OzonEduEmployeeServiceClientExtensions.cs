using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Stubs;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class OzonEduEmployeeServiceClientExtensions
    {
        public static Employee ToEmployee(this OzonEduEmployeeServiceClient.EmployeeViewModel employeeViewModel)
        {
            var employee = new Employee(
                employeeViewModel.Id,
                PersonName.Create(
                    employeeViewModel.FirstName,
                    employeeViewModel.MiddleName,
                    employeeViewModel.LastName),
                Email.Create(employeeViewModel.Email)
            );
            return employee;
        }
    }
}