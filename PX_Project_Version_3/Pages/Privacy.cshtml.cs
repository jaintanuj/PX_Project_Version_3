using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PX_Project_Version_3.Models;
using PX_Project_Version_3.Data;

namespace PX_Project_Version_3.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly PX_Project_Version_3Context _context;

        public PrivacyModel(PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public bool isJudge { get; set; }
        public bool isUser { get; set; }
        public bool isAdmin { get; set; }
        public bool hasTeam { get; set; }
        public bool NoTeam { get; set; }

        public IActionResult OnGet()
        {

            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Index");
            }
            else
            {
                //we make sure that user didn't meddle with the cookies
                User theuser = _context.User.FirstOrDefault(u => u.UserName.Equals(username));
                if (theuser == null)
                {
                    return RedirectToPage("Index");
                }
            }

            //Now before we move forward we have to perform various checks 
            //Such as for admin
            AppCondition app = _context.AppCondition.FirstOrDefault(app => app.AppConditionId.Equals(1));

            int adminid = app.AdminID;
            string adminname = app.AdminName;

            if (adminname.Equals(username))
            {
                //Which would mean that the user is the admin
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }

            //Now we have to check if user is a judge or not
            Judge judge = _context.Judge.FirstOrDefault(j => j.UserName.Equals(username) && j.EventID.Equals(app.EventID));

            if (judge == null)
            {
                //That would mean that user is not a judge
                isJudge = false;
                isUser = true;
            }
            else
            {
                isJudge = true;
                //False because a judge shouldn't be able to create a team
                isUser = false;
                hasTeam = false;
            }

            //Now that we have sorted results based on wether they are user or not
            //We need to check if they have a team or not

            User user = _context.User.FirstOrDefault(u => u.UserName.Equals(username));
           
            if (isUser)
            {
                hasTeam = false;
                Team team = _context.Team.FirstOrDefault(t => t.UserID.Equals(user.UserId) && t.EventID.Equals(app.EventID));
               
                if (team == null)
                {
                    //He/she is a team owner/leader
                    hasTeam = false;
                }
                else
                {
                    hasTeam = true;
                }

                Member member = _context.Member.FirstOrDefault(m => m.UserID.Equals(user.UserId) && m.EventID.Equals(app.EventID));

                if (member == null)
                {
                    hasTeam = false;
                }
                else
                {
                    hasTeam = true;
                }

                return Page();
            }
            else
            {
                return Page();            }         

            //Might have to confirm the admin access rights once more with the group

        }
    }
}
