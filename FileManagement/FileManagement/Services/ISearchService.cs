using sharedfile.Models;

namespace sharedfile.Services
{
    interface ISearchService
    {
        public SearchVM SearchFiles(string fileName, string uploadDayFrom, string uploadDayTo, int currentPage);
    }
}
