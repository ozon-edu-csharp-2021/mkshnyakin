using System;

namespace OzonEdu.MerchandiseService.Infrastructure.Exceptions
{
    public class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException(string? message) : base(message)
        {
        }
    }
}