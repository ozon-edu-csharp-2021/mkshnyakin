using System;
using System.Text.Json.Serialization;

namespace CSharpCourse.EmployeesService.PresentationModels
{
    public class ColumnKeywordFilter<TColumnIdEnum> where TColumnIdEnum : Enum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TColumnIdEnum Column { get; set; }

        public string[] Keywords { get; set; }
    }
}
