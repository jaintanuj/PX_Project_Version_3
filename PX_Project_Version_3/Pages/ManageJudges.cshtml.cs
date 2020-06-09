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
    public class ManageJudgesModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public ManageJudgesModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public IList<User> Users { get;set; }
        public IList<Judge> Judges { get; set; }
        public IList<Theme> allThemes { get; set; }
        public string JudgeStatus { get; set; }
        public string ThemeName { get; set; }
        public string ThemeType { get; set; }
        //No need to check the user status as admin 
        //as only admin will be able to access this page

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username != app.AdminName)
            {
                //As only admin should be allowed to make or remove judges
                return RedirectToPage("Privacy");
            }

            allThemes = await _context.Theme.Where(t => t.EventID.Equals(app.EventID)).ToArrayAsync();
            //We just upload those users who are judges within the current event
            //Admin does not need to change the judges from previous events
            Judges = await _context.Judge.Where( j => j.EventID.Equals(app.EventID)).ToListAsync();

            Users = await _context.User.ToListAsync();

            JudgeStatus = "Make-Judge";
            ThemeName = "Non-Judge";
            ThemeType = "Non-Judge";

            return Page();
        }
    }
}
