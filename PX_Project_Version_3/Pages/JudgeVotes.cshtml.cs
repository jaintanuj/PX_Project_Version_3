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
    public class JudgeVotesModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public JudgeVotesModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<Team> Team { get;set; }
        public List<SelectListItem> allThemes { get; set; }
        public IList<User> allUsers { get; set; }
        public IList<Event> allEvents { get; set; }
        public bool isAdmin { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string eventCode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            allEvents = await _context.Event.ToListAsync();
            allUsers = await _context.User.ToListAsync();

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            //We need to get all the judges for the current event
            IList<Judge> allJudges = await _context.Judge.Where(j => j.EventID.Equals(app.EventID)).ToListAsync();

            allThemes = await _context.Theme.Where(t => t.EventID.Equals(app.EventID)).Select(
                a => new SelectListItem
                {
                    Value = a.ThemeId.ToString(),
                    Text = a.ThemeName
                }
                ).ToListAsync();

            Team = await _context.Team.Where(t => t.EventID.Equals(app.EventID)).ToListAsync();

            foreach (var team in Team)
            {
                team.UserID = 0;
            }

            if (Team.Count() == 0)
            {
                //Which means no team has been created for the specific event
                Message = "Unfortunalty no team has been created for this event!!";
                return Page();
            }

            if (app.AdminName.Equals(username))
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }

            foreach (var judge in allJudges)
            {
                //We need to load all the votes each judge has made 
                IList<Vote> allVotes = await _context.Vote.Where(v => v.UserID.Equals(judge.UserID) && v.EventID.Equals(app.EventID)).ToListAsync();

                foreach (var vote in allVotes)
                {
                    foreach (var team in Team)
                    {
                        if (vote.TeamID.Equals(team.TeamId))
                        {
                            //So now that we have found the vote for the team
                            //We wil just increment the vote for each team
                            team.UserID += 1;
                        }
                    }
                }
            }

            return Page();
        }


        public async Task<IActionResult> OnPostView()
        {
            var themeid = Int32.Parse(Request.Form["themeid"]);

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));


            Theme theme = await _context.Theme.FirstOrDefaultAsync(th => th.ThemeId.Equals(themeid));

            if (theme == null)
            {
                return RedirectToPage("Privacy");
            }

            allThemes = await _context.Theme.Where(t => t.EventID.Equals(app.EventID)).Select(
               a => new SelectListItem
               {
                   Value = a.ThemeId.ToString(),
                   Text = a.ThemeName
               }
               ).ToListAsync();

            allEvents = await _context.Event.ToListAsync();
            allUsers = await _context.User.ToListAsync();

            //We need to get all the judges for the current event
            IList<Judge> allJudges = await _context.Judge.Where(j => j.EventID.Equals(app.EventID) && j.ThemeID.Equals(themeid)).ToListAsync();

            Team = await _context.Team.Where(t => t.EventID.Equals(app.EventID) && t.ThemeID.Equals(themeid)).ToListAsync();

            foreach (var team in Team)
            {
                team.UserID = 0;
            }

            if (Team.Count() == 0)
            {
                //Which means no team has been created for the specific event
                Message = "Unfortunalty no team has been created for this event!!";
                return Page();
            }

            string username = HttpContext.Session.GetString("username");

            if (app.AdminName.Equals(username))
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }

            foreach (var judge in allJudges)
            {
                //We need to load all the votes each judge has made 
                IList<Vote> allVotes = await _context.Vote.Where(v => v.UserID.Equals(judge.UserID) && v.EventID.Equals(app.EventID)).ToListAsync();

                foreach (var vote in allVotes)
                {
                    foreach (var team in Team)
                    {
                        if (vote.TeamID.Equals(team.TeamId))
                        {
                            //So now that we have found the vote for the team
                            //We wil just increment the vote for each team
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

            //Since there is possibility of a tie we just get rid of the tie teams from before 
            IList<TieBreaker> ties = await _context.TieBreaker.ToListAsync();

            allEvents = await _context.Event.ToListAsync();
            allUsers = await _context.User.ToListAsync();

            if (ties != null)
            {
                foreach (var tie in ties)
                {
                    _context.TieBreaker.Remove(tie);
                    await _context.SaveChangesAsync();
                }
            }

            if (username.Equals(app.AdminName))
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
                return RedirectToPage("Privacy");
            }

            int highvotes = 0;
            IList<Judge> allJudges = await _context.Judge.Where(j => j.EventID.Equals(app.EventID)).ToListAsync();

            if (Team == null)
            {
                Team = await _context.Team.Where(t => t.EventID.Equals(app.EventID)).ToListAsync();

                foreach (var team in Team)
                {
                    team.UserID = 0;
                }

                if (Team.Count() == 0)
                {
                    //Which means no team has been created for the specific event
                    Message = "Unfortunalty no team has been created for this event!!";
                    return Page();
                }

                if (app.AdminName.Equals(username))
                {
                    isAdmin = true;
                }
                else
                {
                    isAdmin = false;
                }

                foreach (var judge in allJudges)
                {
                    //We need to load all the votes each judge has made 
                    IList<Vote> allVotes = await _context.Vote.Where(v => v.UserID.Equals(judge.UserID) && v.EventID.Equals(app.EventID)).ToListAsync();

                    foreach (var vote in allVotes)
                    {
                        foreach (var team in Team)
                        {
                            if (vote.TeamID.Equals(team.TeamId))
                            {
                                //So now that we have found the vote for the team
                                //We wil just increment the vote for each team
                                team.UserID += 1;
                            }
                        }
                    }
                }
            }

            foreach (var team in Team)
            {
                if (team.UserID >= highvotes)
                {
                    highvotes = team.UserID;
                }
            }

            JudgeWinner judgeWinner = await _context.JudgeWinner.FirstOrDefaultAsync(jw => jw.EventID.Equals(app.EventID));

            if (judgeWinner != null)
            {
                Message = "Winner for the event has been finalized!!";
                isAdmin = true;
                return Page();
            }

            foreach (var team in Team)
            {
                //Now we find how many teams have got the highest votes
                Team newTeam = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(team.TeamId) && t.EventID.Equals(app.EventID));

                if (team.UserID.Equals(highvotes))
                {
                    TieBreaker tieBreaker = new TieBreaker()
                    {
                        UserID = newTeam.UserID,
                        EventID = newTeam.EventID,
                        TeamID = newTeam.TeamId,
                    };

                    //Then we store each team with the highest votes
                    _context.TieBreaker.Add(tieBreaker);
                    await _context.SaveChangesAsync();
                }
            }

            //Now we perform checks wether there is a tie breaker or not
            //For that we load all the teams which have been aded for the event
            IList<TieBreaker> tieBreakers = await _context.TieBreaker.Where(tie => tie.EventID.Equals(app.EventID)).ToListAsync();

            if (tieBreakers.Count() == 1)
            {
                Team team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(tieBreakers[0].TeamID) && t.EventID.Equals(tieBreakers[0].EventID));

                JudgeWinner judgeWinner1 = new JudgeWinner()
                {
                    EventID = team.EventID,
                    TeamID = team.TeamId,
                    TeamName = team.TeamName,
                    UserID = team.UserID
                };
                _context.JudgeWinner.Add(judgeWinner1);
                await _context.SaveChangesAsync();
                //Now that we have saved the winner for the event to the judge winner list 
                //Now we just have to delete the tie breaker before we exit

                _context.TieBreaker.Remove(tieBreakers[0]);
                await _context.SaveChangesAsync();

                return RedirectToPage("JudgeWinnerList");
            }
            else
            {
                return RedirectToPage("TieBreaakForJudgeWinner");
            }
        }
    }
}
