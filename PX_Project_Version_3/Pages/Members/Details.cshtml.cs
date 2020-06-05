using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.Members
{
    public class DetailsModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public DetailsModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public Member Member { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Member = await _context.Member.FirstOrDefaultAsync(m => m.MemberId == id);

            if (Member == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
