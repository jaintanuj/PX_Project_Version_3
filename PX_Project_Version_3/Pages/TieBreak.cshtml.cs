using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PX_Project_Version_3.Pages
{
    public class TieBreakModel : PageModel
    {
        private readonly PX_Project_Version_3.Data.PX_Project_Version_3Context _context;

        public TieBreakModel(PX_Project_Version_3.Data.PX_Project_Version_3Context context)
        {
            _context = context;
        }

        public IList<TieBreaker> TieBreaker { get; set; }

        public async Task OnGetAsync()
        {
            TieBreaker = await _context.TieBreaker.ToListAsync();
        }
    }
}
