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
    public class TieBreaakForJudgeWinnerModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public TieBreaakForJudgeWinnerModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<TieBreaker> TieBreakers { get;set; }
        public IList<User> allUsers { get; set; }
        public string UserName { get; set; }
        public IList<Team> allTeams { get; set; }
        public string TeamName { get; set; }
        public IList<Event> allEvents { get; set; }
        public string EventCode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username != app.AdminName)
            {
                return RedirectToPage("Privacy");
            }

            TieBreakers = await _context.TieBreaker.ToListAsync();

            if (TieBreakers.Count() == 0)
            {
                //Which shouldn't be the case if admin has come through following the steps
                return RedirectToPage("JudgeVotes");
            }

            allEvents = await _context.Event.ToListAsync();
            allTeams = await _context.Team.Where(t => t.EventID.Equals(app.EventID)).ToListAsync();
            allUsers = await _context.User.ToListAsync();

            return Page();
        }
    }
}
