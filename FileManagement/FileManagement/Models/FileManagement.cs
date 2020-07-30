using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sharedfile.Models
{
    public class FileManagement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string GUID { get; set; }
        public string FileName { get; set; }
        public string FileUniqueName { get; set; }
        public string BlobUrl { get; set; }
        public DateTime UploadedDate { get; set; }
        public float FileSize { get; set; }
        public bool DeleteFlag { get; set; }
    }
}