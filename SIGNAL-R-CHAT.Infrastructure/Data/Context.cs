using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SIGNAL_R_CHAT.Domain;
using System.Reflection;


namespace SIGNAL_R_CHAT.Infrastructure
{
    public class Context : DbContext 
    {
        public Context (DbContextOptions<Context> options)
            :base(options)
        {
        }
        public DbSet<WorkGroup> WorkGroups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Person> Persons { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }

}
