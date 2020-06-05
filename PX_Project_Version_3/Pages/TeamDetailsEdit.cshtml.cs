using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages
{
    public class TeamDetailsEditModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public TeamDetailsEditModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; }
        public IList<Team> AllTeams { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            User loginUser = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));

            Team = await _context.Team.FirstOrDefaultAsync(t => t.UserID.Equals(loginUser.UserId) && t.EventID.Equals(app.EventID));

            //That is because we are trying to find the login user current team if he/she has any

            if (Team == null)
            {
                //This is because only leaders can edit team details
                return RedirectToPage("MyTeam");
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            AllTeams = await _context.Team.Where(t => t.EventID.Equals(app.EventID)).ToListAsync();

           

            //Before we save we need to make sure team name is unique among the team for a specific event
            foreach (var team in AllTeams)
            {
                if (team.TeamName.Equals(Team.TeamName))
                {
                    Message = "Team Name has to be Unique!! Try Again!!";
                    return Page();
                }
            }

            if (ModelState.IsValid)
            {
                Team team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(Team.TeamId) && t.EventID.Equals(app.EventID));
                team.TeamName = Team.TeamName;
                team.ProjectName = Team.ProjectName;
                team.Idea = Team.Idea;
                await _context.SaveChangesAsync();
                return RedirectToPage("MyTeam");
            }

            return RedirectToPage("./MyTeam");
        }
    }
}
