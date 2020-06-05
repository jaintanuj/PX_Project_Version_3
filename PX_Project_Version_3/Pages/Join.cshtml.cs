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
    public class JoinModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public JoinModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public string EventCode {get;set;}
        public bool InLimit { get; set; }
        //The member limit is just used to make sure only restricted number of people can join a team
        //It will be true when the number of members is equal to less than the recommended by admin

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                //That means user hasn't logged-in
                return RedirectToPage("Privacy");
            }

            if (id == null)
            {
                return NotFound();
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            //This is to make sure user is trying to join teams from the current event only
            Team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(id) && t.EventID.Equals(app.EventID));

            if (Team == null)
            {
                return RedirectToPage("JoinTeam");
            }

            //Now we make sure that user is neither leader nor member for the current event
            User loginuser = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));

            Team team = _context.Team.FirstOrDefault(t => t.UserID.Equals(loginuser.UserId) && t.EventID.Equals(app.EventID));
            if (team != null)
            {
                //That would mean he/she already has a team
                //So we just send him/her to my team page
                return RedirectToPage("MyTeam");
            }

            Member member = _context.Member.FirstOrDefault(m => m.UserID.Equals(loginuser.UserId) && m.EventID.Equals(app.EventID));

            if (member != null)
            {
                //That would mean user has a team
                //So we send him/her there
                return RedirectToPage("MyTeam");
            }

            Event eve = await _context.Event.FirstOrDefaultAsync(e => e.EventId.Equals(app.EventID));
            EventCode = eve.EventCode;

            User leader = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(Team.UserID));
            Email = leader.Email;

            IList<Member> members = await _context.Member.Where(m => m.TeamID.Equals(Team.TeamId) && m.EventID.Equals(app.EventID)).ToListAsync();
            int totalMemberofTeam = members.Count();
            int totalAllowedMembers = app.MemberPerTeam;

            if (totalMemberofTeam >= totalAllowedMembers)
            {
                //That is a team for a given event has maximum occupany achieved
                InLimit = false;
                Message = "This team has reached maximum occupany!! Try Another Team";
            }
            else
            {
                //That is team is within the maximum occupany 
                InLimit = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {

            string username = HttpContext.Session.GetString("username");

            if (id == null)
            {
                return RedirectToPage("JoinTeam");
            }

            var joinCode = Request.Form["joinCode"];

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            Team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(id) && t.EventID.Equals(app.EventID));

            User loginUser = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));

            if (Team != null)
            {
                //We have to make sure that the code is correct
                if (Team.JoinCode != joinCode)
                {
                    Message = "The join Code is Incorrect!! Try Again!!";
                    return Page();
                }
                //else just add the user to the respective team
                Member newMember = new Member()
                {
                    UserID = loginUser.UserId,
                    TeamID = Team.TeamId,
                    Email = loginUser.Email,
                    EventID = Team.EventID,
                    FullName = loginUser.FullName,
                    UserName = loginUser.UserName
                };

                _context.Member.Add(newMember);
                await _context.SaveChangesAsync();
                return RedirectToPage("MyTeam");
            }

            return RedirectToPage("./JoinTeam");
        }
    }
}
