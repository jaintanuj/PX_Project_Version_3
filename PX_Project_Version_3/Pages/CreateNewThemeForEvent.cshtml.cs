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
    public class CreateNewThemeForEventModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public CreateNewThemeForEventModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app = _context.AppCondition.FirstOrDefault(app => app.AppConditionId.Equals(1));

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

            //Now we need to firstly upload the event id from appCondition

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            Event Event = await _context.Event.FirstOrDefaultAsync(e => e.EventId.Equals(app.EventID));

            if (Event == null)
            {
                //This means something went wrong 
                //We should send the admin to the events page to make sure 
                //That the event is there
                return RedirectToPage("AllEvents");
            }
            else
            {
                //We firslty need to save this them
                Theme.EventID = Event.EventId;
                Theme.EventCode = Event.EventCode;
                Theme.EventName = Event.EventName;

                _context.Theme.Add(Theme);
                await _context.SaveChangesAsync();
            }

            //Now we need to update the theme id of the condition table with this theme

            Theme theme = await _context.Theme.FirstOrDefaultAsync(th => th.EventID.Equals(app.EventID));

            if (theme == null)
            {
                //Theme has either not been saved or been deleted
                return RedirectToPage("AllThemes");
            }
            else
            {
                //Now we need to update condition table theme with this event theme
                app.ThemeID = theme.ThemeId;
                await _context.SaveChangesAsync();

                //Now we need to allow the admin to change the app condition 
                return RedirectToPage("EventParametersUpdate");
            }

        }
    }
}
