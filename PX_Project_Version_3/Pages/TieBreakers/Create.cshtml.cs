using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.TieBreakers
{
    public class CreateModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public CreateModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public TieBreaker TieBreaker { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TieBreaker.Add(TieBreaker);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
