using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.JudgeWinners
{
    public class DeleteModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public DeleteModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public JudgeWinner JudgeWinner { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            JudgeWinner = await _context.JudgeWinner.FirstOrDefaultAsync(m => m.JudgeWinnerId == id);

            if (JudgeWinner == null)
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

            JudgeWinner = await _context.JudgeWinner.FindAsync(id);

            if (JudgeWinner != null)
            {
                _context.JudgeWinner.Remove(JudgeWinner);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
