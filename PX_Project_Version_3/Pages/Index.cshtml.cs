using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PX_Project_Version_3.Data;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PX_Project_Version_3Context _context;

        public IndexModel(PX_Project_Version_3Context context)
        {
            _context = context;
        }

        [BindProperty]
        public User user { get; set; }
        public string Message { get; set; }

        public IActionResult OnGet()
        {
            string username = HttpContext.Session.GetString("username");
            if (username != null)
            {
                //Then we check if the value is valid
                User newUser = _context.User.FirstOrDefault(u => u.UserName.Equals(username));

                if (newUser != null)
                {
                    //That means user can login now
                    return RedirectToPage("Privacy");
                }
            }
            user = new User();
            return Page();
        }

        public User Login(string username, string password)
        {
            User newuser = _context.User.FirstOrDefault(u => u.UserName.Equals(username));
            if (newuser == null)
            {
                Message = "Invalid username or password";
                return null;
            }
            else
            {
                if (newuser.Password.Equals(password))
                {
                    return user;
                }
                else
                {
                    Message = "Invalid username or password";
                }
            }
            return null;
        }

        public IActionResult OnPost()
        {
            var theuser = Login(user.UserName, user.Password);
            if (theuser == null)
            {
                return Page();
            }
            else
            {
               //Have to fix the one given below
              //  User newuser =  await _context.User.FirstOrDefault(u );
                HttpContext.Session.SetInt32("userid", theuser.UserId);
                HttpContext.Session.SetString("username", theuser.UserName);
                return RedirectToPage("Privacy");
            }
        }
    }
}
