using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PX_Project_Version_3.Models;

namespace PX_Project_Version_3.Data
{
    public class PX_Project_Version_3Context : DbContext
    {
        public PX_Project_Version_3Context (DbContextOptions<PX_Project_Version_3Context> options)
            : base(options)
        {
        }

        public DbSet<PX_Project_Version_3.Models.User> User { get; set; }

        public DbSet<PX_Project_Version_3.Models.Team> Team { get; set; }

        public DbSet<PX_Project_Version_3.Models.AppCondition> AppCondition { get; set; }

        public DbSet<PX_Project_Version_3.Models.Event> Event { get; set; }

        public DbSet<PX_Project_Version_3.Models.Judge> Judge { get; set; }

        public DbSet<PX_Project_Version_3.Models.Member> Member { get; set; }

        public DbSet<PX_Project_Version_3.Models.Theme> Theme { get; set; }

        public DbSet<PX_Project_Version_3.Models.Vote> Vote { get; set; }

        public DbSet<PX_Project_Version_3.Models.VoteCount> VoteCount { get; set; }

        public DbSet<PX_Project_Version_3.Models.TieBreaker> TieBreaker { get; set; }

        public DbSet<PX_Project_Version_3.Models.PeopleWinner> PeopleWinner { get; set; }

        public DbSet<PX_Project_Version_3.Models.JudgeWinner> JudgeWinner { get; set; }
    }
}
