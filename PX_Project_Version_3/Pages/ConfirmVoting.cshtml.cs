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
    public class ConfirmVotingModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public ConfirmVotingModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public User loginUser { get; set; }
        public string Email { get; set; }
        public string Voter { get; set; }
        public string VoteTo { get; set; }
        public Team selectedTeam { get; set; }
        public IList<Vote> userVotes { get; set; }
        public string Message { get; set; }
        public string button { get; set; }
        public string EventCode { get; set; }
        public string EventName { get; set; }

        //Here id is teamid of which user is not a member of 
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            loginUser = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));
            Voter = loginUser.FullName;
            Email = loginUser.Email;

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            Event eve = await _context.Event.FirstOrDefaultAsync(e => e.EventId.Equals(app.EventID));
            EventCode = eve.EventCode;
            EventName = eve.EventName;

            if (id == null)
            {
                return RedirectToPage("Voting");
            }

            selectedTeam = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(id) && t.EventID.Equals(app.EventID));

            if (selectedTeam == null)
            {
                //That is no such teams exist
                return RedirectToPage("Voting");
            }

             userVotes =await _context.Vote.Where(v => v.UserID.Equals(loginUser.UserId) && v.EventID.Equals(app.EventID)).ToListAsync();

            //This is when there are no votes that the user has cast so far
            VoteTo = selectedTeam.TeamName;

            if (userVotes.Count() == 0)
            {
                Message = "Are you sure you want to vote for this team";
                button = "Vote";
            }

            foreach (var vote in userVotes)
            {
                //WE are looking if user has voted for the team or not
                if (vote.TeamID.Equals(selectedTeam.TeamId))
                {
                    //This would mean that the user has already voted for this team
                    Message = "Are you sure you wish to unovte?";
                    button = "Unvote";
                    return Page() ;
                }
                Message = "Are you sure you want to vote for this tema";
                button = "Vote";

            }

            //We need to check if user is trying to vote beyong the limit set by admin
            if (button == "Vote")
            {
                //That means user is trying to vote
                //WE have to check the user votes and see if he/she has reached limit
                if (userVotes.Count() >= app.VotesAllowed)
                {
                    //Limit has been reached
                    //return RedirectToPage("Voting");

                    //So now that the user has made a vote we need to send him back 
                    //Based on his identity
                     username = HttpContext.Session.GetString("username");

                    IList<Judge> allJudges = await _context.Judge.Where(j => j.EventID.Equals(app.EventID)).ToListAsync();

                    User user = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));

                    foreach (var judge in allJudges)
                    {
                        if (judge.UserID.Equals(user.UserId))
                        {
                            return RedirectToPage("JudgeVoting");
                        }
                    }

                    //Other wise the user is just a normal user
                    //So we just send him/her back to confirm voting
                    return RedirectToPage("Voting");

                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            //Now we have to check wether to vote or unovte
            //We will vote if a user hasn't vote for that team yet

            //IList<Vote> allUserVotes = await _context.Vote.Where(v => v.UserName.Equals(username) && v.EventID.Equals(app.EventID)).ToListAsync();

            //foreach (var vote in allUserVotes)
            //{
            //    //Now we run through the entire loop if 
            //    //The team with teamid = id is found in the list 
            //    //Then we unvote it

            //    if (vote.TeamID.Equals(id))
            //    {
            //        //WE have found the vote
            //        _context.Vote.Remove(vote);
            //        await _context.SaveChangesAsync();
            //    }

            //}

            Vote vote = await _context.Vote.FirstOrDefaultAsync(v => v.UserName.Equals(username) && v.TeamID.Equals(id) && v.EventID.Equals(app.EventID));

            Team team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(id));
            User user = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));

            if (vote == null)
            {
                //That means user wish to vote for this team
                Vote newVote = new Vote()
                {
                    EventID = app.EventID,
                    TeamID = team.TeamId,
                    TeamName = team.TeamName,
                    UserID = user.UserId,
                    UserName = user.UserName
                };
                _context.Vote.Add(newVote);
                await _context.SaveChangesAsync();
                


            }
            else{
                //Then we will have to unvote this team as user has already votes fo4r this team
                _context.Vote.Remove(vote);
                await _context.SaveChangesAsync();
             }


            IList<Judge> allJudges = await _context.Judge.Where(j => j.EventID.Equals(app.EventID)).ToListAsync();
            //Before we send the user back we need to the voting
            //We need to perform certain checks for that
            foreach (var judge in allJudges)
            {
                if (judge.UserID.Equals(user.UserId))
                {
                    return RedirectToPage("JudgeVoting");
                }
            }

            //Other wise the user is just a normal user
            //So we just send him/her back to confirm voting
            return RedirectToPage("Voting");
        }
    }
}
