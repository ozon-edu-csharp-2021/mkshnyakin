using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;
using Xunit;

namespace OzonEdu.MerchandiseService.Domain.Tests.Models
{
    public class EnumerationTests
    {
        private sealed class NoElement : Enumeration
        {
            public NoElement(int id, string name) : base(id, name)
            {
            }
        }

        private sealed class BadElement : Enumeration
        {
            public static object One = new();

            public BadElement(int id, string name) : base(id, name)
            {
            }
        }

        private sealed class MoreThanOneId : Enumeration
        {
            public static MoreThanOneId OneId = new(1, nameof(OneId));
            public static MoreThanOneId TwoId = new(2, nameof(TwoId));
            public static MoreThanOneId OneMoreId = new(1, nameof(OneMoreId));

            public MoreThanOneId(int id, string name) : base(id, name)
            {
            }
        }

        [Fact]
        public void GetById_ThrowsCorruptedValueObjectException_WhenNoElement()
        {
            Assert.Throws<CorruptedValueObjectException>(() => Enumeration.GetById<NoElement>(1));
        }

        [Fact]
        public void GetById_ThrowsCorruptedValueObjectException_WhenBadElement()
        {
            Assert.Throws<CorruptedValueObjectException>(() => Enumeration.GetById<BadElement>(1));
        }

        [Fact]
        public void GetById_ThrowsCorruptedValueObjectException_WhenMoreThanOneId()
        {
            Assert.Throws<CorruptedValueObjectException>(() => Enumeration.GetById<MoreThanOneId>(1));
        }
    }
}