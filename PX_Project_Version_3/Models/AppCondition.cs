using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class AppCondition
    {
        public int AppConditionId { get; set; }
        public int AdminID { get; set; }
        public string AdminName { get; set; }
        //The event id will show the current event or the prevailing
        public int EventID { get; set; }
        public int ThemeID { get; set; }
        public int VotesAllowed { get; set; }
        public int MemberPerTeam { get; set; }
        public int AppStatus { get; set; }
    }
}
