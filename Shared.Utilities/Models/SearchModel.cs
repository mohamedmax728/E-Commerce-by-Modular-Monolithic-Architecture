namespace Shared.Utilities.Models.Models
{
    public class SearchModel
    {
        public string? OrderBy { get; set; }
        public string SortOrder { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Filter { get; set; }
        public bool? AllPages { get; set; } = false;
    }
}
