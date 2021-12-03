using System;

namespace CSharpCourse.EmployeesService.PresentationModels
{
    public interface IIdModel<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
