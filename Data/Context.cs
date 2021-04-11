using Microsoft.EntityFrameworkCore;
using MacintoshBED.Models;

namespace MacintoshBED.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<User> User { get; set; }
        public DbSet<Candidate> Candidate {get;set;}
        public DbSet<Employer> Employer {get;set;}
        public DbSet<Admin> Admin {get;set;}
    }
}
