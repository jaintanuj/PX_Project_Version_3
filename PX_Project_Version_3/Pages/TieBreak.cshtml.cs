using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Pages
{
    public class TieBreakModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public TieBreakModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<TieBreaker> TieBreaker { get; set; }
        public IList<User> allUsers { get; set; }
        public IList<Event> allEvents { get; set; }
        public IList<Team> allTeams { get; set; }
        public string UserName { get; set; }
        public string EventCode { get; set; }
        public string TeamName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            TieBreaker = await _context.TieBreaker.Where(tie => tie.EventID.Equals(app.EventID)).ToListAsync();

            if (TieBreaker.Count() == 0)
            {
                //The user tried to access it via url possibly
                return RedirectToPage("PeopleVotes");
            }

            allEvents = await _context.Event.ToListAsync();
            allTeams = await _context.Team.Where(t => t.EventID.Equals(app.EventID)).ToListAsync();
            allUsers = await _context.User.ToListAsync();

            return Page();
        }
    }
}
