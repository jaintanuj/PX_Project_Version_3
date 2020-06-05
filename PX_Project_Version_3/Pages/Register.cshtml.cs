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
    public class RegisterModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public RegisterModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            //int userid = (int)HttpContext.Session.GetInt32("userid");
            string username = HttpContext.Session.GetString("username");

            if (username != null)
            {
                //That means user has already logged-in
                //Now we have to check if the login details are real
                User newuser = _context.User.FirstOrDefault(u => u.UserName.Equals(username));
                if (newuser != null)
                {
                    //User and his details have been found
                    return RedirectToPage("./Privacy");
                }
            }

            return Page();
        }

        [BindProperty]
        public User User { get; set; }
        public string Message { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string username = User.UserName.Trim();
            string name = User.FullName.Trim();
            string email = User.Email.Trim();
            string password = User.Password.Trim();

            if (username == "")
            {
                Message = "Username is required!!";
                return Page();
            }

            if (name == "")
            {
                Message = "Name is required!!";
                return Page();
            }

            if (email == "")
            {
                Message = "Email is required!!";
                return Page();
            }

            if (password == "")
            {
                Message = "Password cannot be left empty!!";
                return Page();
            }

            bool usernameCheck = UserNameTaken(User.UserName);
            bool emailCheck = EmailTaken(User.Email);
            if (usernameCheck == true )
            {
                //That means the username is taken
                Message = "UserName is already Taken!!Try Again!!";
                return Page();
            }

            if (emailCheck == true)
            {
                //That means the email is taken
                Message = "Email is already registered!!";
                return Page();
            }

            _context.User.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public bool UserNameTaken(string username)
        {
            User user =  _context.User.FirstOrDefault(u => u.UserName.Equals(username));
            if (user == null)
            {
                //That is username is unique
                return false;
            }
            return true;
        }

        public bool EmailTaken(string email)
        {
            User user = _context.User.FirstOrDefault(u => u.Email.Equals(email));
            if (user == null)
            {
                return false;
            }
            return true;
        }
    }
}
