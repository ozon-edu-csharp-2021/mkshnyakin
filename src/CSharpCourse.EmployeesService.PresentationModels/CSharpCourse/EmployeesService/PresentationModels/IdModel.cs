using System;

namespace CSharpCourse.EmployeesService.PresentationModels
{
    public class IdModel<TKey> : IIdModel<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }
}
