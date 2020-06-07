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
    public class ConfirmJudgeWinnerModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public ConfirmJudgeWinnerModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public TieBreaker TieBreaker { get; set; }
        public string UserName { get; set; }
        public string TeamName { get; set; }
        public string EventCode { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("TieBreaakForJudgeWinner");
            }

            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username != app.AdminName)
            {
                RedirectToPage("Privacy");
            }

            TieBreaker = await _context.TieBreaker.FirstOrDefaultAsync(m => m.TieBreakerId == id);

            Team team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(TieBreaker.TeamID) && t.EventID.Equals(TieBreaker.EventID));
            User user = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(TieBreaker.UserID));
            Event eve = await _context.Event.FirstOrDefaultAsync(eve => eve.EventId.Equals(TieBreaker.EventID));

            if (eve == null || team == null || user == null)
            {
                return RedirectToPage("JudgeVotes");
            }

            if (TieBreaker == null)
            {
                return RedirectToPage("JudgeVotes");
            }

            UserName = user.UserName;
            TeamName = team.TeamName;
            EventCode = eve.EventCode;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TieBreaker tieBreaker = await _context.TieBreaker.FirstOrDefaultAsync(tie => tie.TieBreakerId.Equals(id));

            Team team = await _context.Team.FirstOrDefaultAsync(t => t.TeamId.Equals(tieBreaker.TeamID));

            if (team == null)
            {
                RedirectToPage("JudgeVotes");
            }

            JudgeWinner judgeWinner = new JudgeWinner()
            {
                EventID = tieBreaker.EventID,
                TeamID = tieBreaker.TeamID,
                UserID = tieBreaker.EventID,
                TeamName = team.TeamName
            };

            _context.JudgeWinner.Add(judgeWinner);
            await _context.SaveChangesAsync();

            return RedirectToPage("./JudgeWinnerList");
        }
    }
}
