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
    public class MyTeamModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public MyTeamModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; }
        public User Leader { get; set; }
        public IList<Member> Members { get; set; }
        public bool isLeader { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");


            if (username == null)
            {
                //That is the user hasn't logged-in
                return RedirectToPage("Index");
            }

            //This is get the details of the user who logged-in
            User loginuser = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
     

            //If user is correct
            //Then we check if the user is a leader or a team member

            //1.Leader check for the user i.e if user is the leader
            Team = await _context.Team.FirstOrDefaultAsync(t => t.UserID.Equals(loginuser.UserId) && t.EventID.Equals(app.EventID));
            if (Team == null)
            {
                isLeader = false;
                //Not the leader for the team
                //We need to check if he/she is a member of any team
                Member member = await _context.Member.FirstOrDefaultAsync(m => m.UserID.Equals(loginuser.UserId) && m.EventID.Equals(app.EventID));

                if (member == null)
                {
                    //Since that would mean he/she does not have a team for this event
                    //So we just send him/her back
                    return RedirectToPage("CreateTeam");
                }
                else
                {
                    //This would mean he/she is in a team
                    int teamid = member.TeamID;
                    Members = await _context.Member.Where(t => t.TeamID.Equals(teamid) && t.EventID.Equals(app.EventID)).ToListAsync();
                    //Now we have to load the team
                    Team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(teamid) && t.EventID.Equals(app.EventID));
                    ///Nowe we need to find the leader of the team
                    Leader = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(Team.UserID));
                    //Now we have everything required we just need to go to the page
                    return Page();
                }
            }
            else
            {
                //That means he/she is the leader of his/her team
                isLeader = true;
                Leader = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));
                Members = await _context.Member.Where(m => m.TeamID.Equals(Team.TeamId) && m.EventID.Equals(app.EventID)).ToListAsync();
            }
            return Page();
        }

    }
}
