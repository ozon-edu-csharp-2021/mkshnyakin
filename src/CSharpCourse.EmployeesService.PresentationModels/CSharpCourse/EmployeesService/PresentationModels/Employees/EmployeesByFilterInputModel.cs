using System;
using System.Collections.Generic;
using CSharpCourse.EmployeesService.PresentationModels.Enums;

namespace CSharpCourse.EmployeesService.PresentationModels.Employees
{
    public class EmployeesByFilterInputModel
    {
        public PaginationFilter Paging { get; set; } = new PaginationFilter();


        public Range<DateTime?> HiringDate { get; set; }

        public Range<DateTime?> FiredDate { get; set; }

        public EmployeeFilterStatus EmployeeFilterStatus { get; set; } = EmployeeFilterStatus.Current;


        public IReadOnlyCollection<ColumnKeywordFilter<EmployeeFilteringColumn>> ColumnKeywords { get; set; }
    }
}
