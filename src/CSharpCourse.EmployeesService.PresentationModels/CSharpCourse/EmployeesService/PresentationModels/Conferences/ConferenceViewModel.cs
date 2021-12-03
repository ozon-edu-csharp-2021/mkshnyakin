using System;

namespace CSharpCourse.EmployeesService.PresentationModels.Conferences
{
    public sealed class ConferenceViewModel : IdModel<long>
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }
    }
}
