namespace CourseManagement.Business.DTOs.PaginationDTOs;

public class PageResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int CurrentPageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
