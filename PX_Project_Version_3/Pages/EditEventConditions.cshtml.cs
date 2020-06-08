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
    public class EditEventConditionsModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public EditEventConditionsModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public AppCondition app { get; set; }
        public IList<User> allUsers { get; set; }
        public int totalVotes { get; set; }
        public int totalMembers { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            app = await _context.AppCondition.FirstOrDefaultAsync(m => m.AppConditionId.Equals(1));

            if (app.AdminName != username)
            {
                return RedirectToPage("Privacy");
            }

            if (app == null)
            {
                return RedirectToPage("Privacy");
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //We are not going to recommend 

            int voteLimit = Int32.Parse(Request.Form["votes"]);
            int memberLimit = Int32.Parse(Request.Form["members"]);
            int themeID = Int32.Parse(Request.Form["themeid"]);
            int adminid = Int32.Parse(Request.Form["adminid"]);

            User user = await _context.User.FirstOrDefaultAsync(u => u.UserId.Equals(adminid));
            if (user == null)
            {
                Message = "Something went wrong while updating admin!!";
                return Page();
            }

            string adminname = user.UserName;

            Theme theme = await _context.Theme.FirstOrDefaultAsync(t => t.ThemeId.Equals(themeID));
            if (theme == null)
            {
                Message = "Something went wrong while updating theme!!";
                return Page();
            }

            //For members there are two ways:
            //1. The members will increase which does not matter if the winner has been decided
            //2. The members will decrease which matter cause we have to remove all members
            //As the members will have to removed from their teams
            int oldMembersLimit = app.MemberPerTeam;

            if (memberLimit >= oldMembersLimit)
            {
                //In this scenario we have to make sure winner hasn't been decided yet
                PeopleWinner peopleWinner = await _context.PeopleWinner.FirstOrDefaultAsync(pw => pw.EventID.Equals(app.EventID));

                JudgeWinner judgeWinner = await _context.JudgeWinner.FirstOrDefaultAsync(jw => jw.EventID.Equals(app.EventID));

                if (peopleWinner != null || judgeWinner != null)
                {
                    //That would mean that the winner has already been finalised
                    //So we can just skip it all together
                }
                else
                {
                    //Need to clarify my thoughts before i can proceed
                }
            }
            else
            {
                //This is the case where the members have been 

            }
            
            


            return RedirectToPage("Privacy");
        }

      
    }
}
