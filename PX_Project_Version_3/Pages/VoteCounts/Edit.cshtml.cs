using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.VoteCounts
{
    public class EditModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public EditModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(VoteCount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoteCountExists(VoteCount.VoteCountId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool VoteCountExists(int id)
        {
            return _context.VoteCount.Any(e => e.VoteCountId == id);
        }
    }
}
