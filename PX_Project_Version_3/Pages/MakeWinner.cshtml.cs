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
    public class MakeWinnerModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public MakeWinnerModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Team Team { get; set; }
        public string LeaderName { get; set; }
        public string EventCode { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            if (app.AdminName != username)
            {
                //As only admin decide who can be the winner
                return RedirectToPage("Privacy");
            }

            if (id == null)
            {
                //Which shouldn't be possible if the admin correct path and procedure
                return RedirectToPage("TieBreak");
            }

            TieBreaker tie = await _context.TieBreaker.FirstOrDefaultAsync(tie => tie.TieBreakerId.Equals(id) && tie.EventID.Equals(app.EventID));

            if (tie == null)
            {
                //Which means the tie breaker team is not found
                return RedirectToPage("PeopleVotes");
            }

            Event eve = await _context.Event.FirstOrDefaultAsync(eve => eve.EventId.Equals(tie.EventID));
            EventCode = eve.EventCode;

            Team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(tie.TeamID) && t.EventID.Equals(tie.EventID));


            if (Team == null)
            {
                //Which is not possible unless someone deletes the team
                //In this case we jus send the admin back to the total votes
                return RedirectToPage("PeopleVotes");
            }

            User leader = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(Team.UserID));
            LeaderName = leader.UserName;
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            //Here id refers to tiebreaker id as due to design flaws
            if (id == null)
            {
                return RedirectToPage("PeopleWinnerList");
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            TieBreaker tie = await _context.TieBreaker.FirstOrDefaultAsync(tie => tie.TieBreakerId.Equals(id) && tie.EventID.Equals(app.EventID));

            if (tie == null)
            {
                //Something obviosuly went wrong
                return RedirectToPage("PeopleVotes"); 
            }

            Team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(tie.TeamID) && t.EventID.Equals(tie.EventID));

            if (Team.TeamId != tie.TeamID)
            {
                //Which shouldn't be the case considering the OnGet function
                return RedirectToPage("PeopleVotes");
            }

            PeopleWinner peopleWinner = new PeopleWinner()
            {
                EventID = Team.EventID,
                TeamID = Team.TeamId,
                TeamName = Team.TeamName,
                UserID = Team.UserID
            };

            _context.PeopleWinner.Add(peopleWinner);
            await _context.SaveChangesAsync();

            //Now that winner has been found we need to remove the tie breaker teams
            IList<TieBreaker> tieBreakers = await _context.TieBreaker.ToListAsync();

            foreach (var ties in tieBreakers)
            {
                _context.TieBreaker.Remove(ties);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./PeopleWinnerList");
        }
    }
}
