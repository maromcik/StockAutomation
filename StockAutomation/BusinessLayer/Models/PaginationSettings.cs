namespace BusinessLayer.Models;

public class PaginationSettings
{
    public int PageSize { get; }
    public int PageNumber { get; }
    
    public PaginationSettings(int pageSize, int pageNumber)
    {
        PageSize = pageSize;
        PageNumber = pageNumber;
    }
    
    
}