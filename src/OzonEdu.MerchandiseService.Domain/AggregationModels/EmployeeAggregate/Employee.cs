using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate
{
    public sealed class Employee : Entity
    {
        public Employee(PersonName personName, Email email)
        {
            Name = personName ?? throw new CorruptedInvariantException($"{nameof(personName)} is null");
            Email = email ?? throw new CorruptedInvariantException($"{nameof(email)} is null");
        }

        public Employee(long id, PersonName name, Email email) : this(name, email)
        {
            Id = id;
        }

        public PersonName Name { get; }

        public Email Email { get; }

        public override string ToString()
        {
            return $"{Id}. {Name}. {Email}";
        }
    }
}