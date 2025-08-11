namespace OperationalMicroservice.Data.DTOs
{
    public class HistoryQueryParams
    {
        // Filters
        public int? Year { get; set; }
        public int? Month { get; set; } // 1-12
        public int? Day { get; set; }   // 1-31

        // Sorting: accept "asc" or "desc" (case-insensitive)
        public string? SortByDate { get; set; } // "asc" | "desc"
        public string? SortBySum { get; set; }  // "asc" | "desc"

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
