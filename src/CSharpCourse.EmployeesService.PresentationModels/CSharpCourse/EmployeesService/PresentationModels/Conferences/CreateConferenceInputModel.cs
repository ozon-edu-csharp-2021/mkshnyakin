using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpCourse.EmployeesService.PresentationModels.Conferences
{
    public sealed class CreateConferenceInputModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Description { get; set; }
    }
}
