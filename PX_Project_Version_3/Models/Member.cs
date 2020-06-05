using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }
        public int UserID { get; set; }
        public int TeamID { get; set; }
        public int EventID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
