using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate
{
    public sealed class Employee : Entity
    {
        public Employee(PersonName name, Email email)
        {
            Name = name;
            Email = email;
        }

        public Employee(long id, PersonName name, Email email) : this(name, email)
        {
            Id = id;
        }

        public PersonName Name { get; }

        public Email Email { get; private set; }

        public void ChangeEmail(string newEmail)
        {
            Email = Email.Create(newEmail);
        }

        public override string ToString()
        {
            return $"{Id}. {Name}. {Email}";
        }
    }
}