using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sharedfile.Models
{
    public class Folder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string GUID { get; set; }
        public string FolderName { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeleteFlag { get; set; }
    }
}
