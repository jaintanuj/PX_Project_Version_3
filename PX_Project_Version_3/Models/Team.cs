using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        [Required]
        public string TeamName { get; set; }
        public string ProjectName { get; set; }
        public string Idea { get; set; }
        public int UserID { get; set; }
        public int EventID { get; set; }
        public string JoinCode { get; set; }
        public int ThemeID { get; set; }
    }
}
