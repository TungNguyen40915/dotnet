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


        public List<Ffile> SearchFiles(string fileName, string uploadDayFrom, string uploadDayTo, string userId)
        {
            List<Ffile> files = new List<Ffile>();

            if (!validate(fileName, uploadDayFrom, uploadDayTo, userId)) throw new Exception();

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
                              where fromTime.Date <= file.UploadedDate.Date && toTime.Date >= file.UploadedDate.Date && file.UserId == userId && file.DeleteFlag == false
                              orderby file.UploadedDate descending
                              select file);

                files = result.ToList();
            }
            else
            {
                var result = (from file in _context.Files
                              where file.FileName.Contains(fileName) && fromTime.Date <= file.UploadedDate.Date && toTime.Date >= file.UploadedDate.Date && file.UserId == userId && file.DeleteFlag == false
                              orderby file.UploadedDate descending
                              select file);
                files = result.ToList();
            }

            return files;
        }


        private bool validate(string fileName, string uploadDayFrom, string uploadDayTo, string userId)
        {
            if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(uploadDayFrom) && string.IsNullOrEmpty(uploadDayTo) && string.IsNullOrEmpty(userId)) return false;


            return true;
        }

        public IEnumerable<Ffile> GetFiles(string userId, string folderId, int number = 10)
        {
            IEnumerable<Ffile> list = Enumerable.Empty<Ffile>();

            if (string.IsNullOrEmpty(folderId))
            {
                list = from file in _context.Files
                       where file.UserId == userId && file.DeleteFlag == false && file.FolderGUID == null
                       orderby file.UploadedDate descending
                       select file;
            }
            else
            {
                list = from file in _context.Files
                       where file.UserId == userId && file.DeleteFlag == false && file.FolderGUID == folderId
                       orderby file.UploadedDate descending
                       select file;
            }

            if (number != 0)
            {
                return list.Take(number).ToList();
            }
            else return list.ToList();

        }
    }
}
