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
    public class JoinTeamModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public JoinTeamModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<Team> Team { get;set; }
        public IList<User> Users { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public  IActionResult OnGet()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }
            AppCondition app = _context.AppCondition.FirstOrDefault(app => app.AppConditionId.Equals(1));
            User loginuser = _context.User.FirstOrDefault(u => u.UserName.Equals(username));
            //We need to make sure that he/she is not part of any other team for the current event
            Team team = _context.Team.FirstOrDefault(t => t.UserID.Equals(loginuser.UserId) && t.EventID.Equals(app.EventID));
            if (team != null)
            {
                //That would mean he/she already has a team
                //So we just send him/her to my team page
                return RedirectToPage("MyTeam");
            }

            Member member = _context.Member.FirstOrDefault(m => m.UserID.Equals(loginuser.UserId) && m.EventID.Equals(app.EventID));

            if (member != null)
            {
                //That would mean user has a team
                //So we send him/her there
                return RedirectToPage("MyTeam");
            }
            Users = _context.User.ToList();
            Team = _context.Team.Where(t => t.EventID.Equals(app.EventID)).ToList();

            return Page();
        }
    }
}
