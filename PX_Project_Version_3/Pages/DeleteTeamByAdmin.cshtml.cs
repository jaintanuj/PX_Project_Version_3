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
    public class DeleteTeamByAdminModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public DeleteTeamByAdminModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; }
        public string Leader { get; set; }
        public string FullName { get; set; }
        public string EventCode { get; set; }
        public IList<Event> allEvents { get; set; }
        public IList<User> allUsers { get; set; }
        public int totalMembers { get; set; }
        public int totalVotes { get; set;}
        public bool hasPresentation { get; set; }
        public string FileName { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("AllTeams");
            }

            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            //We need to make sure that the team exists
            Team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(id));

            if (Team == null)
            {
                return RedirectToPage("AllTeams");
            }

            allEvents = await _context.Event.ToListAsync();
            allUsers = await _context.User.ToListAsync();
            Leader = "Not-Found";
            FullName = "Not-Found";
            EventCode = "Not-Found";

            foreach (var user in allUsers)
            {
                if (Team.UserID.Equals(user.UserId))
                {
                    Leader = user.UserName;
                    FullName = user.FullName;
                    break;
                }
            }

            foreach (var eve in allEvents)
            {
                if (Team.EventID.Equals(eve.EventId))
                {
                    EventCode = eve.EventCode;
                    break;
                }
            }

            IList<Member> allMembers = await _context.Member.Where(m => m.TeamID.Equals(Team.TeamId) && m.EventID.Equals(Team.EventID)).ToListAsync();
            //We need to add plus 1 as for admin the leader is also a members
            totalMembers = allMembers.Count() + 1;

            IList<Vote> teamVotes = await _context.Vote.Where(v => v.TeamID.Equals(Team.TeamId)).ToListAsync();
            totalVotes = teamVotes.Count();

            TeamPresentation teamPresentation = await _context.TeamPresentation.FirstOrDefaultAsync(tp => tp.TeamID.Equals(Team.TeamId));
            if (teamPresentation != null)
            {
                hasPresentation = true;
                FileName = teamPresentation.FileName;
            }
            else
            {
                hasPresentation = false;
                FileName = "None";
            }

            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Team = await _context.Team.FindAsync(id);

            if (Team == null)
            {
                return RedirectToPage("Privacy");
            }

            //Now that we are removing the team from the application database 
            //we need to remove the traces of it as well otherwise they are likely to mess up with the 
            //user experience and data management

            //1. We remove all the members of this team
            IList<Member> allMembers = await _context.Member.Where(m => m.TeamID.Equals(Team.TeamId)).ToListAsync();

            foreach (var mem in allMembers)
            {
                _context.Member.Remove(mem);
                await _context.SaveChangesAsync();
            }

            //2.We need to get rid of all the votes for this team
            IList<Vote> allVotes = await _context.Vote.Where(v => v.TeamID.Equals(Team.TeamId)).ToListAsync();

            foreach (var vote in allVotes)
            {
                _context.Vote.Remove(vote);
                await _context.SaveChangesAsync();
            }

            //3. We need to get rid of the presentation
            TeamPresentation teamPresentation = await _context.TeamPresentation.FirstOrDefaultAsync(tp => tp.TeamID.Equals(Team.TeamId));

            if (teamPresentation != null)
            {
                //Before we remove from the table
                //We need to remove it from the Files folder

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

            //Now we just remove the team as a final step

            _context.Team.Remove(Team);
            await _context.SaveChangesAsync();

            return RedirectToPage("./AllTeams");
        }
    }
}
