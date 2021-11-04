using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;

namespace OzonEdu.MerchandiseService.Infrastructure.Stubs
{
    public class OzonEduEmployeeServiceClient : IOzonEduEmployeeServiceClient
    {
        public class EmployeeViewModel
        {
            public long Id { get; init; }
            public string FirstName { get; init; }
            public string LastName { get; init; }
            public string MiddleName { get; init; }
            public DateTime BirthDay { get; init; }
            public DateTime HiringDate { get; init; }
            public string Email { get; init; }
        }

        private static readonly ImmutableArray<EmployeeViewModel> Items = ImmutableArray.Create(new EmployeeViewModel[]
        {
            new()
            {
                Id = 1,
                FirstName = "Антон",
                MiddleName = "Фёдорович",
                LastName = "Глушков",
                BirthDay = DateTime.Parse("08/18/1978"),
                HiringDate = DateTime.Parse("01/01/2000"),
                Email = "ololo1@example.com"
            },
            new()
            {
                Id = 2,
                FirstName = "Даниил",
                MiddleName = "Иванович",
                LastName = "Гуляев",
                BirthDay = DateTime.Parse("03/15/1979"),
                HiringDate = DateTime.Parse("02/02/2001"),
                Email = "ololo2@example.com"
            },
            new()
            {
                Id = 3,
                FirstName = "Мирон",
                MiddleName = "Мирон",
                LastName = "Завьялов",
                BirthDay = DateTime.Parse("04/16/1980"),
                HiringDate = DateTime.Parse("03/03/2003"),
                Email = "ololo3@example.com"
            },
            new()
            {
                Id = 4,
                FirstName = "Тимур",
                MiddleName = "Николаевич",
                LastName = "Трофимов",
                BirthDay = DateTime.Parse("05/17/1981"),
                HiringDate = DateTime.Parse("04/04/2004"),
                Email = "ololo4@example.com"
            },
            new()
            {
                Id = 5,
                FirstName = "Мирослава",
                MiddleName = "Степановна",
                LastName = "Смирнова",
                BirthDay = DateTime.Parse("06/18/1982"),
                HiringDate = DateTime.Parse("05/05/2005"),
                Email = "ololo5@example.com"
            },
            new()
            {
                Id = 6,
                FirstName = "Иван",
                MiddleName = "Георгиевич",
                LastName = "Захаров",
                BirthDay = DateTime.Parse("07/19/1983"),
                HiringDate = DateTime.Parse("06/06/2006"),
                Email = "ololo6@example.com"
            },
            new()
            {
                Id = 7,
                FirstName = "Виктория",
                MiddleName = "Никитична",
                LastName = "Колесникова",
                BirthDay = DateTime.Parse("08/20/1984"),
                HiringDate = DateTime.Parse("07/07/2007"),
                Email = "ololo7@example.com"
            },
            new()
            {
                Id = 8,
                FirstName = "Ева",
                MiddleName = "Фёдоровна",
                LastName = "Никитина",
                BirthDay = DateTime.Parse("09/21/1985"),
                HiringDate = DateTime.Parse("08/08/2008"),
                Email = "ololo8@example.com"
            },
            new()
            {
                Id = 9,
                FirstName = "Марк",
                MiddleName = "Кириллович",
                LastName = "Куликов",
                BirthDay = DateTime.Parse("10/22/1986"),
                HiringDate = DateTime.Parse("09/09/2009"),
                Email = "ololo9@example.com"
            },
            new()
            {
                Id = 10,
                FirstName = "Мария",
                MiddleName = "Никитична",
                LastName = "Быкова",
                BirthDay = DateTime.Parse("11/23/1987"),
                HiringDate = DateTime.Parse("10/10/2010"),
                Email = "ololo10@example.com"
            },
        });

        public Task<EmployeeViewModel> GetByIdAsync(int employeeId)
        {
            var result = Items.FirstOrDefault(x => x.Id == employeeId);
            return Task.FromResult(result);
        }

        public Task<EmployeeViewModel> FindByEmailAsync(string employeeEmail)
        {
            var result = Items.FirstOrDefault(x => x.Email == employeeEmail);
            return Task.FromResult(result);
        }
    }
}