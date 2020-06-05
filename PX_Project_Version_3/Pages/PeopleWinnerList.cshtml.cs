using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages
{
    public class PeopleWinnerListModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public PeopleWinnerListModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<PeopleWinner> PeopleWinner { get;set; }
        public List<SelectListItem> eventList { get; set; } 
        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                return RedirectToPage("Privacy");
            }

            eventList = await _context.Event.Select(eve => new SelectListItem{
                        Value = eve.EventCode,
                        Text = eve.EventCode}).ToListAsync();

            PeopleWinner = await _context.PeopleWinner.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Here i will take a look at drop down filtering the people winner by eventid
            

            

            eventList = await _context.Event.Select(eve => new SelectListItem
            {
                Value = eve.EventCode,
                Text = eve.EventCode
            }).ToListAsync();

            var code = Request.Form["eventid"];
            Event eve = await _context.Event.FirstOrDefaultAsync(eve => eve.EventCode.Equals(code));
            PeopleWinner = await _context.PeopleWinner.Where(peo => peo.EventID.Equals(eve.EventId)).ToListAsync();

            if (PeopleWinner == null)
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
