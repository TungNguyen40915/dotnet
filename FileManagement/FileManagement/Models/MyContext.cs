using FileManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace sharedfile.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        { }
        public DbSet<Ffile> Files { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ffile>().ToTable("File").Property(p => p.GUID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Log>().ToTable("Log").Property(p => p.GUID).ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().ToTable("User").Property(p => p.GUID).ValueGeneratedOnAdd();
        }
    }
}