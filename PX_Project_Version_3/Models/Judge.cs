using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class Judge
    {
        [Key]
        public int JudgeId { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int EventID { get; set; }
        public int ThemeID { get; set; }
    }
}
