using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace sharedfile.Services.Imp
{
    public class SearchServiceImp : ISearchService
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public SearchServiceImp(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        public SearchVM SearchFiles(string fileName, string uploadDayFrom, string uploadDayTo, int currentPage)
        {
            List<Ffile> files = new List<Ffile>();


            if (!validate(fileName, uploadDayFrom, uploadDayTo)) throw new Exception();

            DateTime fromTime;
            DateTime toTime;

            if (string.IsNullOrEmpty(uploadDayFrom))
                fromTime = new DateTime();
            else
                fromTime = DateTime.Parse(uploadDayFrom);


            if (string.IsNullOrEmpty(uploadDayTo))
                toTime = DateTime.Now;
            else
                toTime = DateTime.Parse(uploadDayTo);

            if (fromTime > toTime) throw new Exception();

            if (string.IsNullOrEmpty(fileName))
            {
                var result = (from file in _context.Files
                              where fromTime.Date <= file.UploadedDate.Date && toTime.Date >= file.UploadedDate.Date
                              orderby file.UploadedDate descending
                              select file);

                files = result.ToList();
            }
            else
            {
                var result = (List<Ffile>)(from file in _context.Files
                                           where file.FileName.Contains(fileName) && fromTime.Date <= file.UploadedDate.Date && toTime.Date >= file.UploadedDate.Date
                                           orderby file.UploadedDate descending
                                           select file);
                files = result.ToList();
            }

            return paging(files, currentPage);
        }


        private SearchVM paging(List<Ffile> files, int currentPage)
        {

            int maxItems = _config.GetValue<int>(Constants.MAX_ITEMS_SEARCH_PAGE);
            int start = (currentPage - 1) * maxItems;
            SearchVM vm = new SearchVM();

            vm.files = files.ToList().Skip(start).Take(maxItems).ToList();
            vm.currentPage = currentPage;
            vm.maxItemsSearchPage = maxItems;
            vm.totalRecords = files.Count();
            vm.pageCount = Convert.ToInt32(Math.Ceiling(files.Count() / (double)maxItems));
            return vm;
        }


        private bool validate(string fileName, string uploadDayFrom, string uploadDayTo)
        {
            if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(uploadDayFrom) && string.IsNullOrEmpty(uploadDayTo)) return false;


            return true;
        }
    }
}
