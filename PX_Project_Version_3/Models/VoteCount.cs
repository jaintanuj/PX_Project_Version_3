using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class VoteCount
    {
        [Key]
        public int VoteCountId { get; set; }
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public int EventID { get; set; }
        public int TotalCounts { get; set; }
    }
}
