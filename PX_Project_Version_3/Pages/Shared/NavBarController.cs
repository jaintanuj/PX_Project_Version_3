using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PX_Project_Version_3.Pages.Shared
{
    public class NavBarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}