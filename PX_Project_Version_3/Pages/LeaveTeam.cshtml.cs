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
    public class LeaveTeamModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public LeaveTeamModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; }
        public string Email { get; set; }
        public string EventCode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            User loginuser = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));

            //This is to make sure user is just trying to remove his current team
            Member member = await _context.Member.FirstOrDefaultAsync(m => m.UserID.Equals(loginuser.UserId) && m.EventID.Equals(app.EventID));

            if (member == null)
            {
                //This would mean he doesn't have a team for the current event
                return RedirectToPage("MyTeam");
            }

            //That means user does have a team
            //So we will go forward with the processing
            Event eve = await _context.Event.FirstOrDefaultAsync(e => e.EventId.Equals(app.EventID));
            Team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(member.TeamID) && t.EventID.Equals(member.EventID));
            User leader = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(Team.UserID));
            Email = leader.Email;
            EventCode = eve.EventCode;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            //Now we have to find the user's memberId to remove it from the current team
            Member member = await _context.Member.FirstOrDefaultAsync(m => m.UserName.Equals(username) && m.EventID.Equals(app.EventID));

            if (member != null)
            {
                _context.Member.Remove(member);
                await _context.SaveChangesAsync();
                return RedirectToPage("JoinTeam");
            }

            return RedirectToPage("./Privacy");
        }
    }
}
