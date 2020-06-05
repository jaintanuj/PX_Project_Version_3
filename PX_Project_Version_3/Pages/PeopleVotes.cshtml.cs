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
    public class PeopleVotesModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public PeopleVotesModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<Team> Team { get;set; }
        public bool isAdmin { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            Team = await _context.Team.Where(t => t.EventID.Equals(app.EventID)).ToListAsync();
            IList<User> Users = await _context.User.ToListAsync();

            foreach (var team in Team)
            {
                //WE will be using Userid as a place to store the total votes for a team
                team.UserID = 0;
            }

            User loginUser = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));

            if (app.AdminName.Equals(username))
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }

            foreach (var user in Users)
            {
                //Firstly we need to fetch all the vote users has for this session
                IList<Vote> userVotes = await _context.Vote.Where(v => v.UserID.Equals(user.UserId) && v.EventID.Equals(app.EventID)).ToListAsync();

                //Next based on the vote we assign or increment each vote by one based on the respective user votes 
                foreach (var vote in userVotes)
                {
                    foreach (var team in Team)
                    {
                        if (vote.TeamID.Equals(team.TeamId))
                        {
                            //That is the user has voted for this team in the event 
                            //So we just increament the team vote by one
                            team.UserID += 1;
                        }
                    }
                }
            }


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string username = HttpContext.Session.GetString("username");

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username.Equals(app.AdminName))
            {
                isAdmin = true;
            }
            else
            {
                //Since only admin is supposed to use this functionality we send the other user back
                isAdmin = false;
                return RedirectToPage("Privacy");
            }

            int highVotes = 0;

            if (Team ==  null)
            {
                Team = await _context.Team.Where(t => t.EventID.Equals(app.EventID)).ToListAsync();
                IList<User> Users = await _context.User.ToListAsync();

                foreach (var team in Team)
                {
                    //WE will be using Userid as a place to store the total votes for a team
                    team.UserID = 0;
                }

                User loginUser = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));

                if (app.AdminName.Equals(username))
                {
                    isAdmin = true;
                }
                else
                {
                    isAdmin = false;
                }

                foreach (var user in Users)
                {
                    //Firstly we need to fetch all the vote users has for this session
                    IList<Vote> userVotes = await _context.Vote.Where(v => v.UserID.Equals(user.UserId) && v.EventID.Equals(app.EventID)).ToListAsync();

                    //Next based on the vote we assign or increment each vote by one based on the respective user votes 
                    foreach (var vote in userVotes)
                    {
                        foreach (var team in Team)
                        {
                            if (vote.TeamID.Equals(team.TeamId))
                            {
                                //That is the user has voted for this team in the event 
                                //So we just increament the team vote by one
                                team.UserID += 1;
                            }
                        }
                    }
                }
            }

            foreach (var team in Team)
            {
                if (team.UserID >= highVotes)
                {
                    highVotes = team.UserID;
                }
            }

            PeopleWinner peopleWinner = await _context.PeopleWinner.FirstOrDefaultAsync(peopleWin => peopleWin.EventID.Equals(app.EventID));

            if (peopleWinner != null)
            {
                Message = "People Winner has already been decided before!! If you wish to change remove that team first";
                isAdmin = true;
                return Page();
            }

            //Now that we have to the highest votes we need to find the teams which have recieve high votes
            foreach (var team in Team)
            {
                //We store the team details of each team in a dummy team object for later use
                Team newTeam = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(team.TeamId) && t.EventID.Equals(app.EventID));

                if (team.UserID.Equals(highVotes))
                {
                    TieBreaker tie = new TieBreaker()
                    {
                        UserID = newTeam.UserID,
                        EventID = newTeam.EventID,
                        TeamID = newTeam.TeamId,
                    };

                    //Now that we know that a team has got highest votes we need to store it within tie breaker
                    _context.TieBreaker.Add(tie);
                    await _context.SaveChangesAsync();
                }
            }
            
            //Now we load the team which have got the highest votes for the current event
            //So we check if we have a tie by checkingthe tie team for the current event
            IList<TieBreaker> ties = await _context.TieBreaker.Where(tie => tie.EventID.Equals(app.EventID)).ToListAsync();

            if (ties.Count() == 1)
            {
                Team team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(ties[0].TeamID) && t.EventID.Equals(ties[0].EventID));
                PeopleWinner people = new PeopleWinner()
                {
                    EventID = ties[0].EventID,
                    TeamID = ties[0].TeamID,
                    TeamName =team.TeamName,
                    UserID = ties[0].UserID
                };
                _context.PeopleWinner.Add(people);
                await _context.SaveChangesAsync();

                //Now that we know that we don't have a tie breaker for people winner
                //We will just have to remove the tie breaker teams
                IList<TieBreaker> tiebreakers = await _context.TieBreaker.Where(tie => tie.EventID.Equals(app.EventID)).ToListAsync();
                foreach (var tie in tiebreakers)
                {
                    _context.TieBreaker.Remove(tie);
                    await _context.SaveChangesAsync();
                }

                return RedirectToPage("PeopleWinnerList");
            }

            //Have to change it to Peoples winners page
            return RedirectToPage("./Privacy");
        }
    }
}
