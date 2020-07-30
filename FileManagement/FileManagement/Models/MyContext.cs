using Microsoft.EntityFrameworkCore;

namespace sharedfile.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        { }
        public DbSet<FileManagement> FileManagements { get; set; }
        public DbSet<Log> Logs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileManagement>().ToTable("FileManagement").Property(p => p.GUID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Log>().ToTable("Log").Property(p => p.GUID).ValueGeneratedOnAdd();
        }
    }
}