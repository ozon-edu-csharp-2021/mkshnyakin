using System;
using System.Collections.Generic;
using System.Globalization;
using OzonEdu.MerchandiseService.Domain.Exceptions;
using OzonEdu.MerchandiseService.Domain.Models;

namespace OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate
{
    public sealed class Date : ValueObject
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private Date(DateTime dateTime)
        {
            Value = dateTime;
        }

        public DateTime Value { get; }

        public static Date Create(string dateString)
        {
            return IsValid(dateString, out var dateTime)
                ? new Date(dateTime)
                : throw new CorruptedValueObjectException($"{nameof(Date)} is invalid. Date: {dateString}");
        }

        public static Date Create(DateTime dateTime)
        {
            return new Date(dateTime);
        }

        public override string ToString()
        {
            return $"Date: {Value}";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private static bool IsValid(string dateString, out DateTime dateTime)
        {
            return DateTime.TryParse(dateString, Culture, DateTimeStyles.None, out dateTime);
        }
    }
}