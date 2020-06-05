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
    public class RemoveMemberModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public RemoveMemberModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Member Member { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("MyTeam");
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            Member = await _context.Member.FirstOrDefaultAsync(m => m.MemberId.Equals(id));

            if (Member == null)
            {
                return RedirectToPage("MyTeam");
            }


            int teamid = Member.TeamID;
            Team team = await _context.Team.FirstOrDefaultAsync(t=>t.TeamId.Equals(teamid) && t.EventID.Equals(app.EventID));
            User leader = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(team.UserID));

            if (leader.UserName == username)
            {
                return Page();
            }
            
            return RedirectToPage("MyTeam");
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("MyTeam");
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));
            Member = await _context.Member.FirstOrDefaultAsync(m => m.MemberId.Equals(id) && m.EventID.Equals(app.EventID));

            if (Member != null)
            {
                _context.Member.Remove(Member);
                await _context.SaveChangesAsync();
                return RedirectToPage("JoinTeam");
            }

            return RedirectToPage("./Index");
        }
    }
}
