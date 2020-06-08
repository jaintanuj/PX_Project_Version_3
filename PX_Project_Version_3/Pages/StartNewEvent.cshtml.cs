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
    public class StartNewEventModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public StartNewEventModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app =  _context.AppCondition.FirstOrDefault(a => a.AppConditionId.Equals(1));

            if (username != app.AdminName)
            {
                return RedirectToPage("Privacy");
            }

            return Page();
        }

        [BindProperty]
        public Event Event { get; set; }
        public string Message { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (Event.EventCode.Trim() == "")
            {
                Message = "Event Code cannot be empty!!";
                return Page();
            }

            Event eve = await _context.Event.FirstOrDefaultAsync(eve => eve.EventCode.Equals(Event.EventCode));

            if (eve != null)
            {
                Message = "The event Code has to be unique!! try again";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Event.Add(Event);
            await _context.SaveChangesAsync();

            //Now we need to update the app event id with this new id

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            Event newEvent = await _context.Event.FirstOrDefaultAsync(e => e.EventCode.Equals(Event.EventCode));

            //Now the event id must be updated with the new event created
            app.EventID = newEvent.EventId;
            await _context.SaveChangesAsync();

            return RedirectToPage("./CreateNewThemeForEvent");
        }
    }
}
