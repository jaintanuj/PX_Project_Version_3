using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.Themes
{
    public class DeleteModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public DeleteModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Theme Theme { get; set; }
        public Event Event { get; set; }
        public bool isAdmin { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("/Privacy");
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username == app.AdminName)
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }

            if (id == null)
            {
                return NotFound();
            }

            Theme = await _context.Theme.FirstOrDefaultAsync(m => m.ThemeId == id);
            Event = await _context.Event.FirstOrDefaultAsync(i => i.EventId.Equals(Theme.EventID));

            if (Theme == null)
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

            Theme = await _context.Theme.FindAsync(id);

            if (Theme != null)
            {
                _context.Theme.Remove(Theme);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
