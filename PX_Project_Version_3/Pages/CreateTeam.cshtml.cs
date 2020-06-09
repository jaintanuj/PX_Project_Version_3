using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages
{
    public class CreateTeamModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public CreateTeamModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            User user = _context.User.FirstOrDefault(u => u.UserName.Equals(username));
            if (username == null)
            {
                return Page();
            }
            else
            {

                if (user == null)
                {
                    return RedirectToPage("Index");
                }
            }
            // User user = _context.User.FirstOrDefault(u => u.UserName.Equals(username));
            //Now that everything regarding the user is checked we check if he/she already own a team 
            //Or that he/she is a member of another team with same event
            AppCondition app = _context.AppCondition.FirstOrDefault(app => app.AppConditionId.Equals(1));
            Team team = _context.Team.FirstOrDefault(t => t.UserID.Equals(user.UserId) && t.EventID.Equals(app.EventID));

            //Firsly check for the team
            if (team != null)
            {
                //That would means he/she has a team
                return RedirectToPage("Privacy");
            }

            allThemes = await _context.Theme.Where(t => t.EventID.Equals(app.EventID)).Select(a => new SelectListItem
            {
                Value = a.ThemeId.ToString(),
                Text = a.ThemeName
            }).ToListAsync();


            Member member = _context.Member.FirstOrDefault(m => m.UserID.Equals(user.UserId) && m.EventID.Equals(app.EventID));

            //Now we have to check if he/she is a team member
            if (member != null)
            {
                //That would mean he/she is a team member for the current event
                return RedirectToPage("Privacy");
            }

            return Page();
        }

        [BindProperty]
        public Team Team { get; set; }
        public string Message { get; set; }
        public List<SelectListItem> allThemes{get;set;}

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //Now we add userid, event id and joincode over here

            string username = HttpContext.Session.GetString("username");
            User user = _context.User.FirstOrDefault(u => u.UserName.Equals(username));

            var themeid = Int32.Parse(Request.Form["theme"]);
            ///Which will not be null as we checked earlier in get function
            AppCondition app = _context.AppCondition.FirstOrDefault(app => app.AppConditionId.Equals(1));
          
            Team.UserID = user.UserId;
            Team.EventID = app.EventID;

            //Since we are going tp later create a join link for members
            //We need to make sure that the team joincode is unique
            //For a given team for a current event

            IList<Team> teams = _context.Team.Where(t => t.EventID.Equals(app.EventID)).ToList();

            foreach (var team in teams)
            {
                if (team.TeamName.Equals(Team.TeamName))
                {
                    //That is team name is already taken
                    Message = "The team name already exist!!Try Again!!";
                    return Page();
                }
            }

            //Not sure about the event id for now 
            //Just have to perform the checks on it
            allThemes = await _context.Theme.Where(t => t.EventID.Equals(app.EventID)).Select(a => new SelectListItem {
                Value = a.ThemeId.ToString(),
                Text = a.ThemeName
            }).ToListAsync();

            bool PasswordExist = true;
            while (PasswordExist)
            {
                RandomNumberGenerator random = new RandomNumberGenerator();
                string joincode = random.RandomPassword();
                PasswordExist = false;

                foreach (var team in teams)
                {
                    if (team.JoinCode.Equals(joincode))
                    {
                        PasswordExist = true;
                        break;
                    }
                }

                Team.JoinCode = joincode;
            }

            Team.ThemeID = themeid;

            _context.Team.Add(Team);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Privacy");
        }
    }
    public class RandomNumberGenerator
    {
        // Generate a random number between two numbers    
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size and case.   
        // If second parameter is true, the return string is lowercase  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random password of a given length (optional)  
        public string RandomPassword(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }
    }

}
