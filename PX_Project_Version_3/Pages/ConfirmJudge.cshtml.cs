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
    public class ConfirmJudgeModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public ConfirmJudgeModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }
        public List<SelectListItem> allThemes { get; set; }
        public string EventCode { get; set; }
        public string Message { get; set; }
        public string EventName { get; set; }
        public bool isJudge { get; set; }
        public string buttonStatus { get; set; }
        public string ThemeName { get; set; }
        public string ThemeType  { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username != app.AdminName)
            {
                //Since only admin can manage the judges for the event
                return RedirectToPage("Privacy");
            }

            User  = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(id));

            if (User == null)
            {
                //Which is probably because someone tempered with the id value
                return RedirectToPage("ManageJudges");
            }

            isJudge = false;
            Message = "Are you sure you want to make this user a judge?";
            buttonStatus = "Make-Judge";

            //So we just upload all the judges from the current event
            IList<Judge> judges = await _context.Judge.Where(j => j.EventID.Equals(app.EventID)).ToListAsync();
            int themeid = 0;

            foreach (var judge in judges)
            {
                if (judge.UserID.Equals(User.UserId))
                {
                    Message = "Are you sure you want to remove this user from judges position?";
                    buttonStatus = "Remove-Judge";
                    themeid = judge.ThemeID;
                    isJudge = true;
                    break;
                }
            }

            allThemes = await _context.Theme.Where(t => t.EventID.Equals(app.EventID)).Select(
                a => new SelectListItem
                {
                    Value = a.ThemeId.ToString(),
                    Text = a.ThemeName
                }
                ).ToListAsync();

            Event eve = await _context.Event.FirstOrDefaultAsync(eve => eve.EventId.Equals(app.EventID));
            EventCode = eve.EventCode;
            EventName = eve.EventName;

            
            

            if (isJudge)
            {
                Theme theme = await _context.Theme.FirstOrDefaultAsync(t => t.ThemeId.Equals(themeid));

                ThemeName = theme.ThemeName;
                ThemeType = theme.ThemeType;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                //Which shouldn't be the case as only admin can access this page
                return RedirectToPage("ManageJudges");
            }

            

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            Event eve = await _context.Event.FirstOrDefaultAsync(eve => eve.EventId.Equals(app.EventID));
            EventCode = eve.EventCode;
            EventName = eve.EventName;

            //So there are few things to consider before we can assign judges
            //That is to make sure they haven't accidently created teams before they were assigned
            //If they have teams we need to make sure we delete teams and their members

            User user = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(id));

            if (user == null)
            {
                //Which can mean something went wrong
                //So we just send the admin back to the Judges management page
                return RedirectToPage("ManageJudges");
            }

            

            //Now we have to check if admin is trying to either remove or add this user to the judges table

            IList<Judge> judges = await _context.Judge.Where(j => j.EventID.Equals(app.EventID)).ToListAsync();

            foreach (var judge in judges)
            {
                if (judge.UserID.Equals(user.UserId))
                {
                    //Which means the admin is trying to remvoe this user
                    _context.Judge.Remove(judge);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("ManageJudges");
                }
            }

            //So now we make sure to delete their teams
            Team team = await _context.Team.FirstOrDefaultAsync(t => t.UserID.Equals(user.UserId) && t.EventID.Equals(app.EventID));

            if (team != null)
            {
                //We need to check for the members of that team
                IList<Member> teamMemebers = await _context.Member.Where(m => m.TeamID.Equals(team.TeamId) && team.EventID.Equals(app.EventID)).ToListAsync();
                foreach (var mem in teamMemebers)
                {
                    _context.Member.Remove(mem);
                    await _context.SaveChangesAsync();
                }

                //Now we just have to delete the team
                _context.Team.Remove(team);
                await _context.SaveChangesAsync();

                //WE also need to get rid of the votes for the judges team
                //Otherwise those will still be counted when counting total winner
                IList<Vote> allVotes = await _context.Vote.Where(v => v.TeamID.Equals(team.TeamId) && v.EventID.Equals(team.EventID)).ToListAsync();

                foreach (var vote in allVotes)
                {
                    _context.Vote.Remove(vote);
                    await _context.SaveChangesAsync();
                }
            }

            var themeid = Int32.Parse(Request.Form["themeid"]);

            Judge newJudge = new Judge()
            {

                EventID = app.EventID,
                UserID = user.UserId,
                UserName = user.UserName,
                ThemeID = themeid

            };

            _context.Judge.Add(newJudge);
            await _context.SaveChangesAsync();
           
            return RedirectToPage("./ManageJudges");
        }
    }
}
