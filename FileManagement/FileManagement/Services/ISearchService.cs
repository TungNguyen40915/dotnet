using System.Collections.Generic;
using sharedfile.Models;

namespace sharedfile.Services
{
    interface ISearchService
    {
        public List<Ffile> SearchFiles(string fileName, string uploadDayFrom, string uploadDayTo, string userId);

        public IEnumerable<Ffile> GetFiles(string userId, string folderId, int number = 10);
    }
}
