using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace PX_Project_Version_3.Pages.Themes
{
    public class CreateModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public CreateModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        // Grabbing list of events that exists
        public List<Event> EventsList { get; set; }
        public bool isAdmin { get; set; }
        public string Message { get; set; }

        public async Task OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username == app.AdminName)
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }

            EventsList = await _context.Event.ToListAsync();
        }

        [BindProperty]
        public Theme Theme { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            string username = HttpContext.Session.GetString("username");

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
            _context.Theme.Add(Theme);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
