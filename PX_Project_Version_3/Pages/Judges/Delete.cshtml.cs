using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.Judges
{
    public class DeleteModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public DeleteModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Judge Judge { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Judge = await _context.Judge.FirstOrDefaultAsync(m => m.JudgeId == id);

            if (Judge == null)
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

            Judge = await _context.Judge.FindAsync(id);

            if (Judge != null)
            {
                _context.Judge.Remove(Judge);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
