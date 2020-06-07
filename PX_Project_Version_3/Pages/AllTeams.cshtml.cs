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
    public class AllTeamsModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public AllTeamsModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<Team> Team { get;set; }
        public List<SelectListItem> eventList { get; set; }
        public IList<Event> allEvents { get; set; }
        public IList<User> allUsers { get; set; }
        public string UserName { get; set; }
        public string EventCode { get; set; }
        public bool isAdmin { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
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
            
            eventList = await _context.Event.Select(eve => new SelectListItem
            {
                Value = eve.EventCode,
                Text = eve.EventCode
            }).ToListAsync();

            Team = await _context.Team.ToListAsync();
            allEvents = await _context.Event.ToListAsync();
            allUsers = await _context.User.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            eventList = await _context.Event.Select(eve => new SelectListItem
            {
                Value = eve.EventCode,
                Text = eve.EventCode
            }).ToListAsync();

            allEvents = await _context.Event.ToListAsync();
            allUsers = await _context.User.ToListAsync();

            var code = Request.Form["eventid"];

            if (code == "All")
            {
                Team = await _context.Team.ToListAsync();
                return Page();
            }

            Event eve = await _context.Event.FirstOrDefaultAsync(eve => eve.EventCode.Equals(code));
            Team = await _context.Team.Where(peo => peo.EventID.Equals(eve.EventId)).ToListAsync();

            if (Team == null)
            {
                //That is no such team has been stored 
                //Or that event is still going on
                Message = "No Such team Exist for that event!!";
                return Page();
            }
            else
            {
                Message = "";
            }

            return Page();
        }
    }
}
