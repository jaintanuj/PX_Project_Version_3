using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class TeamPresentation
    {
        /// <summary>
        /// This class will be used to store the file Presentations
        /// </summary>
        [Key]
        public int TeamPresentationId { get; set; }
        public int TeamID { get; set; }
        public string FileName { get; set; }
        public string TeamName { get; set; }
        public int EventID { get; set; }
    }
}
