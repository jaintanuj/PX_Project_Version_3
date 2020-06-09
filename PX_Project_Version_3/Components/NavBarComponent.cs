using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PX_Project_Version_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Components
{
    public class NavBarComponent : ViewComponent
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public NavBarComponent(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            int role = 0;
            string username = HttpContext.Session.GetString("username");
            if (username != null)
            {
                AppCondition app = _context.AppCondition.FirstOrDefault(app => app.AppConditionId.Equals(1));
                User user = _context.User.FirstOrDefault(u => u.UserName.Equals(username));

                // Checking whether user is a team leader
                if (_context.Team.Where(t => t.UserID.Equals(user.UserId) && t.EventID.Equals(app.EventID)).Any())
                {
                    role = 1;
                }

                // Checking whether user is a member of a team
                else if (_context.Member.Where(m => m.UserID.Equals(user.UserId) && m.EventID.Equals(app.EventID)).Any())
                {
                    role = 1;
                }

                // Checking whether user is an admin
                else if (_context.AppCondition.Where(i => i.AdminName == username).Any())
                {
                    role = 4;
                }

                // Checking whether user is a judge
                else if (_context.Judge.Where(j => j.UserName.Equals(username) && j.EventID.Equals(app.EventID)).Any())
                {
                    role = 3;
                }

                // Means the user is not apart of any team
                else
                {
                    role = 2;
                }
            }
            return View(role);
        }
    }
}