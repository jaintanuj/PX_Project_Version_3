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
    public class AllJudgesModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public AllJudgesModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<Judge> Judge { get;set; }
        public List<SelectListItem> allEvents { get; set; }
        public IList<Event> Events { get; set; }
        public IList<User> allUsers { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string EventName { get; set; }
        public string EventCode { get; set; }
        public bool isAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            Judge = await _context.Judge.ToListAsync();
           
            allEvents = await _context.Event.Select(a => new SelectListItem
            {   Value = a.EventId.ToString(),
            Text = a.EventCode
            }).ToListAsync();

            Events = await _context.Event.ToListAsync();

            allUsers = await _context.User.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var eventid = Int32.Parse(Request.Form["eventid"]);

            Event eve = await _context.Event.FirstOrDefaultAsync(e => e.EventId.Equals(eventid));
            Events = await _context.Event.ToListAsync();
            if (eve == null)
            {
                return RedirectToPage("Privacy");
            }
            allUsers = await _context.User.ToListAsync();
            allEvents = await _context.Event.Select(a => new SelectListItem { 
                Value = a.EventId.ToString(),
                Text = a.EventCode
            }).ToListAsync();

            if (eve.EventCode == "All")
            {
                Judge = await _context.Judge.ToListAsync();
            }
            else
            {
                Judge = await _context.Judge.Where(j => j.EventID.Equals(eve.EventId)).ToListAsync();
            }

            return Page();
        }
    }
}
