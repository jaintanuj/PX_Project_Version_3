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
        [Required(ErrorMessage = "A Theme Name is required"), MaxLength(50)]
        public string ThemeName { get; set; }
        [Required(ErrorMessage = "A linked event is required")]
        public int EventID { get; set; }
        public string description { get; set; }
    }
}
