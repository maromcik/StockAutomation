namespace BusinessLayer.Models;

public class PaginationSettings(int pageSize, int pageNumber)
{
    public int PageSize { get; } = pageSize;
    public int PageNumber { get; } = pageNumber;
}
