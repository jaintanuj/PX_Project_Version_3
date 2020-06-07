using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PX_Project_Version_3.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Pages
{
    public class UploadTeamPresentationModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        [Obsolete]
        private readonly IHostEnvironment environment;

        [Obsolete]
        public UploadTeamPresentationModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context, IWebHostEnvironment _environment)
        {
            _context = context;
            environment = _environment;
        }

        [BindProperty]
        public Team Team { get; set; }
        public IFormFile UploadFile { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            User user = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));
            Team = await _context.Team.FirstOrDefaultAsync(t => t.UserID.Equals(user.UserId) && t.EventID.Equals(app.EventID));

            if (Team == null)
            {
                //We just send the user back as only team leader should be allowed to upload presentation
                return RedirectToPage("Privacy");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string username = HttpContext.Session.GetString("username");
            User user = await _context.User.FirstOrDefaultAsync(u => u.UserName.Equals(username));
            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            Team = await _context.Team.FirstOrDefaultAsync(t => t.UserID.Equals(user.UserId) && t.EventID.Equals(app.EventID));

            //The team won't be null because of test performed earlier
            if ((UploadFile == null) || (UploadFile.Length == 0))
            {
                Message = "Please select a file first to upload!!";
                return Page();
            }

            //Before we store the file we need to get rid of the previous file if any
            TeamPresentation teamPresentation1 = await _context.TeamPresentation.FirstOrDefaultAsync(tp => tp.TeamID.Equals(Team.TeamId) && tp.EventID.Equals(app.EventID));

            if (teamPresentation1 != null)
            {
                //That means team leader has already upload a file before this
                //So we need to start by deleting that file first
                string storedFile = teamPresentation1.FileName;
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", storedFile);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                    _context.TeamPresentation.Remove(teamPresentation1);
                    await _context.SaveChangesAsync();
                    Message = "Your previous file has been deleted!!";
                }
                else
                {
                }
                
            }

            string extension = Path.GetExtension(UploadFile.FileName);
            //Since team name is unique for an event and username is unique always
            string fileName = Team.JoinCode + username + Team.TeamId + extension;

            var path = Path.Combine(
                      Directory.GetCurrentDirectory(), "Files", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await UploadFile.CopyToAsync(stream);
            }

            Message = "You file has been uploaded!!";

            TeamPresentation teamPresentation = new TeamPresentation()
            {
                TeamID = Team.TeamId,
                EventID = Team.EventID,
                FileName = fileName,
                TeamName = Team.TeamName,
            };

            _context.TeamPresentation.Add(teamPresentation);
            await _context.SaveChangesAsync();

            return RedirectToPage("./MyTeam");
        }
    }
}
