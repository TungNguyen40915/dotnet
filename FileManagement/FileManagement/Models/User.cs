﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace sharedfile.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string GUID { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
