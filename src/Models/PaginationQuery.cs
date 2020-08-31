using Microsoft.AspNetCore.Mvc;

namespace VotRomania.Models
{
    public class PaginationQuery
    {
        [FromQuery(Name = "pageNumber")] public int PageNumber { get; set; } = 1;
        [FromQuery(Name = "pageSize")] public int PageSize { get; set; } = 25;
    }

}