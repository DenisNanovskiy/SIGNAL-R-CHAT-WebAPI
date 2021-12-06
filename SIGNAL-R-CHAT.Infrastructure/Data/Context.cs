using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SIGNAL_R_CHAT.Domain;
   
namespace SIGNAL_R_CHAT.Infrastructure.Data
{
    public class Context : DbContext 
    {
        public Context (DbContextOptions<Context> options)
            :base(options)
        {
        }
        public DbSet<WorkGroup> WorkGroups { get; set; }
        public DbSet<Addressee> Addressees { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Person> Persons { get; set; }

    }
}
