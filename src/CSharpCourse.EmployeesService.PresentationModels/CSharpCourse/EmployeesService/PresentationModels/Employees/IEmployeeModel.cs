using System;
using CSharpCourse.EmployeesService.PresentationModels.Enums;

namespace CSharpCourse.EmployeesService.PresentationModels.Employees
{
    public interface IEmployeeModel
    {
        string FirstName { get; set; }

        string LastName { get; set; }

        string MiddleName { get; set; }

        DateTime BirthDay { get; set; }

        string Email { get; set; }

        ClothingSize ClothingSize { get; set; }
    }
}
