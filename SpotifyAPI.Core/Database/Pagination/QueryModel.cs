using Microsoft.AspNetCore.Mvc;

namespace SpotifyAPI.Core.Database.Pagination;

public class QueryModel
{
    [FromQuery]
    public int Page { get; set; } = 1;

    [FromQuery]
    public int Limit { get; set; } = 20;

    [FromQuery]
    public string Order { get; set; } = "Id";

    [FromQuery]
    public string Filter { get; set; } = string.Empty;
}