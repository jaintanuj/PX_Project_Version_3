using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.VoteCounts
{
    public class DetailsModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public DetailsModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public VoteCount VoteCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            VoteCount = await _context.VoteCount.FirstOrDefaultAsync(m => m.VoteCountId == id);

            if (VoteCount == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
