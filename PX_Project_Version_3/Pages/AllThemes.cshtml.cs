using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages
{
    public class AllThemesModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public AllThemesModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<Theme> Theme { get;set; }
        public IList<Event> allEvents { get; set; }
        public bool isAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            if (app.AdminName.Equals(username))
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }

            //It is because event Code All has been created to just show all events
            allEvents = await _context.Event.Where(e => e.EventCode!="All").ToListAsync();

            Theme = await _context.Theme.ToListAsync();

            return Page();
        }
    }
}
