namespace CSharpCourse.EmployeesService.PresentationModels
{
    public class BasePaginationResponse<T>
    {
        public T[] Items { get; set; }

        public int TotalCount { get; set; }
    }
}
