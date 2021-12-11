namespace SpotifyAPI.Core.Database.Pagination;

public class Paginated<T> where T : class
{
    public IEnumerable<T> Data { get; set; } = Array.Empty<T>();
    public int Page { get; set; }
    public int TotalPages => Convert.ToInt32(Math.Ceiling(Convert.ToDouble(TotalCount) / Convert.ToDouble(Limit)));
    public int TotalCount { get; set; }
    public int Limit { get; set; }
    public string Order { get; set; } = string.Empty;
    public string Filter { get; set; } = string.Empty;
    public int From => Math.Min(Limit * (Page - 1) + 1, TotalCount);
    public int To => Math.Min(From + Limit - 1, TotalCount);
}