namespace TalkingTails.Repository.Helpers;

public class Pagination<T>
{
    public int TotalItemsCount { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageIndex { get; set; } = 1;

    public int TotalPagesCount
    {
        get
        {
            var temp = TotalItemsCount / PageSize;
            if (TotalItemsCount % PageSize == 0)
            {
                return temp;
            }
            return temp + 1;
        }
    }

    /// <summary>
    /// page number start from 1
    /// </summary>
    public bool Next => PageIndex < TotalPagesCount;
    public bool Previous => PageIndex > 1;
    public required ICollection<T> Items { get; set; }
}
