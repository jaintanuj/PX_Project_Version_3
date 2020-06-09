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
    public class EditTeamDetailsModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public EditTeamDetailsModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; }
        public IList<User> allUsers { get; set; }
        public IList<Event> allEvents { get; set; }
        public string Message { get; set; }
        public IList<Team> allTeams { get; set; }
        public string EventCode { get; set; }
        public string UserName { get; set; }
        public int totalMembers { get; set; }
        public bool hasPresentation { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("AllTeams");
            }

            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username != app.AdminName)
            {
                return RedirectToPage("AllTeams");
            }

            Team = await _context.Team.FirstOrDefaultAsync(m => m.TeamId.Equals(id));

            if (Team == null)
            {
                return RedirectToPage("AllTeams");
            }

            allUsers = await _context.User.ToListAsync();
            UserName = "Not-Found";

            allUsers = await _context.User.ToListAsync();
            IList<Member> allMembers = await _context.Member.Where(m => m.TeamID.Equals(Team.TeamId) && m.EventID.Equals(Team.EventID)).ToListAsync();

            totalMembers = allMembers.Count();

            TeamPresentation teamPresentation = await _context.TeamPresentation.FirstOrDefaultAsync(tp => tp.TeamID.Equals(Team.TeamId));
            allEvents = await _context.Event.ToListAsync();

            if (teamPresentation == null)
            {
                hasPresentation = false;
            }
            else
            {
                hasPresentation = true;
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

            
            allUsers = await _context.User.ToListAsync();
            allEvents = await _context.Event.ToListAsync();

            //Before we go and update the data we need to make sure of certain things
            //Team Name if unique within the event

            //We search if there is a team with same name 
            //IList<Team> allTeams = await _context.Team.Where(t => t.EventID.Equals(Team.EventID)).ToListAsync();

            Team team1 = await _context.Team.FirstOrDefaultAsync(t => t.TeamName.Equals(Team.TeamName) && t.EventID.Equals(app.EventID));

            if (team1 != null)
            {
                Message = "Team with same name exists!!";
                allUsers = await _context.User.ToListAsync();
                allEvents = await _context.Event.ToListAsync();
                allTeams = await _context.Team.ToListAsync();
                Team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(Team.TeamId));
                return Page();
            }

            //We prevent the admin from making changes to username, joincode and EventID
            //The reason being they have a lot of dependency and wrong changes can crash the system
            //So for now we just leave it and figure out an alternative for that

            if (ModelState.IsValid)
            {
                Team team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(Team.TeamId) && t.EventID.Equals(app.EventID));
               
                team.ProjectName = Team.ProjectName;
                team.Idea = Team.Idea;
                await _context.SaveChangesAsync();
                return RedirectToPage("AllTeams");
            }

            Message = "Something went wrong!! Please try again!!";
            return Page();
        }

    }
}
