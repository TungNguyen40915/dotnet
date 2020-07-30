using System;
using System.Collections.Generic;
using System.Linq;

namespace sharedfile.Models
{
    public class SearchLogViewModel
    {
        public Log log { get; set; }
        public Ffile file { get; set; }
    }

    public class SearchLogViewModelList
    {
        public IEnumerable<SearchLogViewModel> modelList { get; set; }
        public int pageCount { get; set; }
        public int currentPage { get; set; }

        public int totalRecords { get; set; }
    }
}