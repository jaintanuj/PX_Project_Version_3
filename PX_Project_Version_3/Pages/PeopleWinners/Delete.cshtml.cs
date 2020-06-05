using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.PeopleWinners
{
    public class DeleteModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public DeleteModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public PeopleWinner PeopleWinner { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PeopleWinner = await _context.PeopleWinner.FirstOrDefaultAsync(m => m.PeopleWinnerId == id);

            if (PeopleWinner == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PeopleWinner = await _context.PeopleWinner.FindAsync(id);

            if (PeopleWinner != null)
            {
                _context.PeopleWinner.Remove(PeopleWinner);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
