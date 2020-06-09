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

namespace PX_Project_Version_3.Pages
{
    public class AddNewThemeModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public AddNewThemeModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app =  _context.AppCondition.FirstOrDefault(a => a.AppConditionId.Equals(1));

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            if (username != app.AdminName)
            {
                return RedirectToPage("Privacy");
            }

            return Page();
        }

        [BindProperty]
        public Theme Theme { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(a => a.AppConditionId.Equals(1));
            Event eve = await _context.Event.FirstOrDefaultAsync(e => e.EventId.Equals(app.EventID));

            if (eve == null)
            {
                //Something obviously went wrong 
                //Such as admin might have deleted the event
                return RedirectToPage("Privacy");
            }

            Theme.EventID = eve.EventId;
            Theme.EventCode = eve.EventCode;
            Theme.EventName = eve.EventName;

            _context.Theme.Add(Theme);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Privacy");
        }
    }
}
