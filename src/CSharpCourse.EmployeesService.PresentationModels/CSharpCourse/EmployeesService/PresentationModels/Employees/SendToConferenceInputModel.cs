using CSharpCourse.EmployeesService.PresentationModels.Enums;

namespace CSharpCourse.EmployeesService.PresentationModels.Employees
{
    public sealed class SendToConferenceInputModel
    {
        public long EmployeeId { get; set; }

        public long ConferenceId { get; set; }

        public EmployeeInConferenceType AsWhom { get; set; }
    }
}
