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
    public class JudgeVotingModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public JudgeVotingModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<Team> Team { get;set; }
        public List<SelectListItem> allThemes { get; set; }
        public IList<Event> AllEvents { get; set; }
        public IList<User> AllUsers { get; set; }
        public IList<Vote> UserVotes { get; set; }
        public string VoteStatus { get; set; }
        public string EventCode { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public Theme theme { get; set; }
        public string ThemeName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            VoteStatus = "Vote";

            User loginUser = _context.User.FirstOrDefault(u => u.UserName.Equals(username));
            AppCondition app = _context.AppCondition.FirstOrDefault(app => app.AppConditionId.Equals(1));

            AllEvents = _context.Event.ToList();
            AllUsers = _context.User.ToList();
            //Now we need to check all the votes of the logged-in user for the given event
            UserVotes = _context.Vote.Where(v => v.UserID.Equals(loginUser.UserId) && v.EventID.Equals(app.EventID)).ToList();

            if (UserVotes.Count() >= app.VotesAllowed)
            {
                //That means user has reached his/her to limit to vote for the event
                Message = "You have reached max vote!! You need to unovte a Voted team";
            }
            else
            {
                Message = "You can vote any team except your own!!";
            }

            //Here we need to make sure if a judge tries to vote for a team
            //He/she can only vote a team with the same theme in the same event
              //We need to check if the login user is a judge

            Judge judge = await _context.Judge.FirstOrDefaultAsync(j => j.UserID.Equals(loginUser.UserId));

            //If the user is not a judge we just send it back
            if (judge == null)
            {
                return RedirectToPage("Privacy");
            }

            Team team = _context.Team.FirstOrDefault(t => t.UserID.Equals(loginUser.UserId) && t.EventID.Equals(app.EventID));

            //Firstly we find users team and omit that team from the list
            if (team != null)
            {
                //User does have a team for the event
                Team = _context.Team.Where(t => (t.TeamId != team.TeamId) && t.EventID.Equals(app.EventID) && t.ThemeID.Equals(judge.ThemeID)).ToList();

                 theme = await _context.Theme.FirstOrDefaultAsync(t => t.ThemeId.Equals(judge.ThemeID));
                ThemeName = theme.ThemeName;

                return Page();
            }

            Member member = _context.Member.FirstOrDefault(m => m.UserID.Equals(loginUser.UserId) && m.EventID.Equals(app.EventID));

            if (member != null)
            {
                Team = _context.Team.Where(t => (t.TeamId != member.TeamID) && t.EventID.Equals(app.EventID) && t.ThemeID.Equals(judge.ThemeID)).ToList();

                 theme = await _context.Theme.FirstOrDefaultAsync(t => t.ThemeId.Equals(judge.ThemeID));
                ThemeName = theme.ThemeName;

                return Page();
            }

            //We just load all the teams with same theme AS THE JUDGE
            Team = _context.Team.Where(t => t.EventID.Equals(app.EventID) && t.ThemeID.Equals(judge.ThemeID)).ToList();

             theme = await _context.Theme.FirstOrDefaultAsync(t => t.ThemeId.Equals(judge.ThemeID));
            ThemeName = theme.ThemeName;

            return Page();
               
        }
    }
}
