using System.Collections.Generic;

namespace sharedfile.Models
{
    public class SearchVM
    {
        public List<Ffile> files { get; set; }
        public int pageCount { get; set; }
        public int maxItemsSearchPage { get; set; }
        public int currentPage { get; set; }
        public int totalRecords { get; set; }
    }
}
