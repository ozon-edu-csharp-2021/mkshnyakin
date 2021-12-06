using System;
using System.Collections.Generic;
using CSharpCourse.EmployeesService.PresentationModels.Conferences;
using CSharpCourse.EmployeesService.PresentationModels.Enums;

namespace CSharpCourse.EmployeesService.PresentationModels.Employees
{
    public sealed class EmployeeViewModel : IdModel<long>, IEmployeeModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime BirthDay { get; set; }

        public DateTime HiringDate { get; set; }

        public string Email { get; set; }

        public ClothingSize ClothingSize { get; set; }

        public IReadOnlyCollection<ConferenceViewModel> Conferences { get; set; }
    }
}
