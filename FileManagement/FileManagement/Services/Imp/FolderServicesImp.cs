using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using sharedfile.Models;

namespace sharedfile.Services.Imp
{
    public class FolderServicesImp : IFolderService
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public FolderServicesImp(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        public List<Folder> GetAllFoldersByUserId(string userId)
        {
            List<Folder> list = new List<Folder>();

            try
            {
                list = (from folder in _context.Folders
                        where folder.UserId == userId
                        orderby folder.CreatedDate descending
                        select folder).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return list;
        }

        public Folder GetFolderById(string folderId)
        {
            Folder f = new Folder();

            try
            {
                f = (from folder in _context.Folders
                     where folder.GUID == folderId
                     select folder).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return f;
        }

        public bool CreateFolder(string name, string userId)
        {
            try
            {
                Folder folder = new Folder();
                folder.FolderName = name;
                folder.UserId = userId;
                folder.CreatedDate = System.DateTime.Now;
                folder.DeleteFlag = false;
                _context.Folders.Add(folder);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
                return false;

            }
        }
    }
}
