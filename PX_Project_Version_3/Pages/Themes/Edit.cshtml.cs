using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.Themes
{
    public class EditModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public EditModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Theme Theme { get; set; }
        public List<Event> EventsList { get; set; }
        public bool isAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Checking user's logged in role
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

            if (id == null)
            {
                return NotFound();
            }

            Theme = await _context.Theme.FirstOrDefaultAsync(m => m.ThemeId == id);
            EventsList = await _context.Event.ToListAsync();


            if (Theme == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Checking user's logged in role
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

            if (!ModelState.IsValid)
            {
                EventsList = await _context.Event.ToListAsync();

                return Page();
            }

            _context.Attach(Theme).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThemeExists(Theme.ThemeId))
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

        private bool ThemeExists(int id)
        {
            return _context.Theme.Any(e => e.ThemeId == id);
        }
    }
}
