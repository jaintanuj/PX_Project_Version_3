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
    public class JudgeWinnerListModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public JudgeWinnerListModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<JudgeWinner> JudgeWinner { get;set; }
        public List<SelectListItem> eventList { get; set; }
        public IList<User> allUser { get; set; }
        public IList<Event> allEvents { get; set; }
        public string UserName { get; set; }
        public string EventCode { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            eventList = await _context.Event.Select(eve => new SelectListItem
            {
                Value = eve.EventCode,
                Text = eve.EventCode
            }).ToListAsync();

            JudgeWinner = await _context.JudgeWinner.ToListAsync();

            allUser = await _context.User.ToListAsync();
            allEvents = await _context.Event.ToListAsync();

            UserName = "Not-Found";
            EventCode = "Not-Found";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            eventList = await _context.Event.Select(eve => new SelectListItem
            {
                Value = eve.EventCode,
                Text = eve.EventCode
            }).ToListAsync();

            var code = Request.Form["eventid"];

            allUser = await _context.User.ToListAsync();
            allEvents = await _context.Event.ToListAsync();

            if (code == "All")
            {
                JudgeWinner = await _context.JudgeWinner.ToListAsync();
                return Page();
            }

            Event eve = await _context.Event.FirstOrDefaultAsync(eve => eve.EventCode.Equals(code));
            JudgeWinner = await _context.JudgeWinner.Where(peo => peo.EventID.Equals(eve.EventId)).ToListAsync();

          

            if (JudgeWinner.Count() == 0)
            {
                //That is no such team has been stored 
                //Or that event is still going on
                Message = "No Such team Exist for that event!!";
                return Page();
            }
            else
            {
                Message = "";
            }

            return Page();
        }
    }
}
