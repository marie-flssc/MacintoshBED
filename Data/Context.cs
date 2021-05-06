using Microsoft.EntityFrameworkCore;
using MacintoshBED.Models;

namespace MacintoshBED.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<User> User { get; set; }
        public DbSet<JobDescription> Jobs {get;set;}
        public DbSet<JobProposition> JobsProposition {get;set;}
        public DbSet<Messages> Chat {get;set;}
        public DbSet<MessageBoxe> MessageBox {get;set;}
    }
}
