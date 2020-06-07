using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Pages
{
    public class DeleteUserModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public DeleteUserModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }
        public IList<Vote> allVotes {get;set;}
        public Member teamMember { get; set; }
        public Team team { get; set; }
        public string TeamName { get; set; }
        public bool isLeader { get; set; }
        public bool isMember { get; set; }
        public bool isJudge { get; set; }
        public bool isVoter { get; set; }
        public Judge judge { get; set; }
        public int totalVotes { get; set; }
        public int totalMemberShip { get; set; }
        public string JudgeEvent { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string username = HttpContext.Session.GetString("username");
            
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (!app.AdminName.Equals(username))
            {
                return RedirectToPage("Privacy");
            }
            
            User = await _context.User.FirstOrDefaultAsync(m => m.UserId.Equals(id));

            if (User == null)
            {
                return RedirectToPage("AllTeams");
            }

            team = await _context.Team.FirstOrDefaultAsync(t => t.UserID.Equals(User.UserId));

            if (team != null)
            {
                isLeader = true;
                TeamName = team.TeamName;
            }
            else
            {
                isLeader = false;
            }

            IList<Member> allMembers = await _context.Member.Where(m => m.UserID.Equals(User.UserId)).ToListAsync();

            if (allMembers.Count() == 0)
            {
                isMember = false;
            }
            else
            {
                isMember = true;
                totalMemberShip = allMembers.Count();
            }

            allVotes = await _context.Vote.Where(v => v.UserID.Equals(User.UserId)).ToListAsync();

            if (allVotes.Count() == 0)
            {
                isVoter = false;
            }
            else
            {
                isVoter = true;
                totalVotes = allVotes.Count();
            }

            judge = await _context.Judge.FirstOrDefaultAsync(j => j.UserID.Equals(User.UserId));

            if (judge != null)
            {
                isJudge = true;
                Event eve = await _context.Event.FirstOrDefaultAsync(j => j.EventId.Equals(judge.EventID));
                JudgeEvent = eve.EventCode;
            }
            else
            {
                isJudge = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("AllUsers");
            }

            User = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(id));

            if (User == null)
            {
                return RedirectToPage("AllUsers");
            }

            //WE need to start removing the traces of the users throughout the application
            //1. Let begin by the votes
            allVotes = await _context.Vote.Where(v => v.UserID.Equals(User.UserId)).ToListAsync();

            foreach (var vote in allVotes)
            {
                _context.Vote.Remove(vote);
                await _context.SaveChangesAsync();
            }

            //2. Let remove his membership from various teams if he has any
            IList<Member> allMembersShips = await _context.Member.Where(m => m.UserID.Equals(User.UserId)).ToListAsync();

            foreach (var member in allMembersShips)
            {
                _context.Member.Remove(member);
                await _context.SaveChangesAsync();
            }

            //3. Judges position
            IList<Judge> AsJudge = await _context.Judge.Where(j => j.UserID.Equals(User.UserId)).ToListAsync();

            foreach (var judge in AsJudge)
            {
                _context.Judge.Remove(judge);
                await _context.SaveChangesAsync();
            }

            //4. All Team Leader postion
            IList<Team> allTeams = await _context.Team.Where(t => t.UserID.Equals(User.UserId)).ToListAsync();

            //The code inside the foreach loop is a refrence from the deleteTeamByAdmin Page
            foreach (var team in allTeams)
            {
                Team Team = await _context.Team.FindAsync(id);

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
            }

            //Now in the end we are just gonna remove the users

            _context.User.Remove(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./AllUsers");
        }
    }
}
