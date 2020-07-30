using sharedfile.Models;

namespace sharedfile.Services
{
    /// <summary>
    /// Search Service
    /// </summary>
    interface ISearchService
    {
        public SearchVM SearchFiles(string fileName, string uploadDayFrom, string uploadDayTo, string keyWord, int currentPage);
    }
}
