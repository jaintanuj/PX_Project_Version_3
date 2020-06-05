using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class JudgeWinner
    {
        public int JudgeWinnerId { get; set; }
        public int UserID { get; set; }
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public int EventID { get; set; }
    }
}
