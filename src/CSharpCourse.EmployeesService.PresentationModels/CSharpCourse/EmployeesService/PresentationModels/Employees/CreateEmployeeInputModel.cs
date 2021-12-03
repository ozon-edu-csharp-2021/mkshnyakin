using System;
using System.ComponentModel.DataAnnotations;
using CSharpCourse.EmployeesService.PresentationModels.Enums;

namespace CSharpCourse.EmployeesService.PresentationModels.Employees
{
    public sealed class CreateEmployeeInputModel : IEmployeeModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string MiddleName { get; set; }

        [Required]
        public DateTime BirthDay { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ClothingSize ClothingSize { get; set; }
    }
}
