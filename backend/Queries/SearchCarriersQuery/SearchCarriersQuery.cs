using System;

namespace backend.Queries.SearchCarriersQuery
{
    public class SearchCarriersQuery
    {
        public string? SjpNumber { get; set; }
        public string? CarrierCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Status { get; set; }
    }
}