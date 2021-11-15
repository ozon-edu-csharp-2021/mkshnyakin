using System.Collections.Generic;
using System.Linq;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.AggregationModels.EmployeeAggregate
{
    public class EmailTests
    {
        public static IEnumerable<object[]> ValidEmails => new[]
        {
            "ololo@example.com",
            "ololo@11.example.com",
            "ololo+mailru@example.ru",
            "email@example.com",
            "firstname.lastname@example.com",
            "email@subdomain.example.com",
            "firstname+lastname@example.com",
            "\"email\"@example.com",
            "1234567890@example.com",
            "email@example-one.com",
            "_______@example.com",
            "email@example.name",
            "email@example.museum",
            "email@example.co.jp",
            "firstname-lastname@example.com"
        }.Select(e => new object[] {e});

        public static IEnumerable<object[]> InvalidEmails => new[]
        {
            null,
            "",
            " ",
            "1",
            "-1",
            "@",
            ".",
            "ololo",
            "ololo@",
            "ololo@.",
            "@ololo",
            "@ololo.",
            "plainaddress",
            "#@%^%#$@#$@#.com",
            "@example.com",
            "Joe Smith <email@example.com>",
            "email.example.com",
            "email@example@example.com",
            ".email@example.com",
            "email.@example.com",
            "email..email@example.com",
            "email@example.com (Joe Smith)",
            "email@example",
            "email@-example.com",
            "email@111.222.333.44444",
            "email@example..com",
            "Abc..123@example.com",
            @"”(),:;<>[\]@example.com",
            "this\\ is\"really\"not\\allowed@example.com"
        }.Select(e => new object[] {e});

        [Theory]
        [MemberData(nameof(ValidEmails))]
        public void EmailCreation_ReturnCorrectValueObject_WhenEmailStringIsValid(string emailString)
        {
            var email = Email.Create(emailString);
            Assert.Equal(emailString, email.Value);
        }

        [Theory]
        [MemberData(nameof(InvalidEmails))]
        public void EmailCreation_ThrowsCorruptedValueObjectException_WhenEmailStringIsInvalid(string emailString)
        {
            Assert.Throws<CorruptedValueObjectException>(() => Email.Create(emailString));
        }
    }
}