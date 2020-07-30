using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sharedfile.Models
{
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string GUID { get; set; }
        public string UserId { get; set; }
        public string Division { get; set; }
        public DateTime OperationDate { get; set; }
        public string FileAreaGUID { get; set; }
        public string FileGUID { get; set; }
        public string LogType { get; set; }
        public string ErrorLogTrace { get; set; }
        public bool DeleteFlag { get; set; }

    }
}