using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OzonEdu.MerchandiseService.Domain.Exceptions;

namespace OzonEdu.MerchandiseService.Domain.Models
{
    public abstract class Enumeration : IComparable
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<T>();

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration) other).Id);

        public static T GetById<T>(int id) where T : Enumeration
        {
            T result;
            try
            {
                result = GetAll<T>().SingleOrDefault(x => x.Id == id);
            }
            catch (InvalidOperationException e)
            {
                throw new CorruptedValueObjectException($"More than one {typeof(T).Name} found for id={id}", e);
            }
            catch (Exception e)
            {
                throw new CorruptedValueObjectException(e.Message, e);
            }
            
            return result
                   ?? throw new CorruptedValueObjectException($"{typeof(T).Name} not found for id={id}");
        }
    }
}