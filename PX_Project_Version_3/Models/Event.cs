using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        [Required]
        public string EventCode { get; set; }
        public string EventName { get; set; }
    }
}
