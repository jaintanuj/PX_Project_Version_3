﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages.Events
{
    public class IndexModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public IndexModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<Event> Event { get;set; }
        public bool isAdmin { get; set; }


        public async Task OnGetAsync()
        {
            string username = HttpContext.Session.GetString("username");

            if (username == null)
            {
                 RedirectToPage("/Privacy");
            }

            AppCondition app = await _context.AppCondition.FirstOrDefaultAsync(app => app.AppConditionId.Equals(1));

            if (username == app.AdminName)
            {
                isAdmin = true;
            }
            else
            {
                isAdmin = false;
            }

            Event = await _context.Event.ToListAsync();
        }
    }
}
