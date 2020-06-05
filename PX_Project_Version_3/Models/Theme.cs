using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class Theme
    {
        [Key]
        public int ThemeId { get; set; }
        public string ThemeName { get; set; }
        public string ThemeType { get; set; }
        public int EventID { get; set; }
        public string EventName { get; set; }
        public string EventCode { get; set; }
    }
}
