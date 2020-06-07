using System;
using System.Collections.Generic;
using System.IO;
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
    public class DeleteTeamModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public DeleteTeamModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("MyTeam");
            }

            //Firstly we need to check wether the user is a leader or nor
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            User loginUser = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));

            Team = await _context.Team.FirstOrDefaultAsync(t => t.UserID.Equals(loginUser.UserId) && t.EventID.Equals(app.EventID));

            if (Team == null)
            {
                //User is not a leader or team owner
                return RedirectToPage("MyTeam");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            int teamid = Team.TeamId;
            //To delete a team we need to remove all the member of that team
            IList<Member> teamMembers = await _context.Member.Where(m => m.TeamID.Equals(teamid) && m.EventID.Equals(app.EventID)).ToListAsync();

            foreach (var member in teamMembers)
            {
                _context.Member.Remove(member);
                await _context.SaveChangesAsync();
            }

            //Now that team has been deleted we have to remove votes for that team in the event
            IList<Vote> teamVotes = await _context.Vote.Where(v => v.TeamID.Equals(Team.TeamId) && v.EventID.Equals(app.EventID)).ToListAsync();

            foreach (var vote in teamVotes)
            {
                _context.Vote.Remove(vote);
                await _context.SaveChangesAsync();
            }

            //we also need to remove the presentation of the team if any
            TeamPresentation teamPresentation = await _context.TeamPresentation.FirstOrDefaultAsync(tp => tp.TeamID.Equals(Team.TeamId));

            if (teamPresentation != null)
            {

                //We need to remove the file from the folder as well.
                string storedFile = teamPresentation.FileName;
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", storedFile);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                    _context.TeamPresentation.Remove(teamPresentation);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.TeamPresentation.Remove(teamPresentation);
                    await _context.SaveChangesAsync();
                }

               
            }

            _context.Team.Remove(Team);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Privacy");
        }
    }
}
