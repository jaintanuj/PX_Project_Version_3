using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class TieBreaker
    {
        [Key]
        public int TieBreakerId { get; set; }
        public int TeamID { get; set; }
        public int UserID { get; set; }
        public int EventID { get; set; }
    }
}
