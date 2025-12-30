using LibraryMvcApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;   // ✔️ مهم
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
 

    }
   
    public DbSet<Book> Books { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<FormEntry> FormEntries { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>()
                .HasIndex(d => d.Code)
                .IsUnique();

            // Seeding
            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Code = 53,
                    Name = "إدارة الجودة",
                    StartFormNumber = 200
                },
                new Department
                {
                    Id = 2,
                    Code = 50,
                    Name = "إدارة السلامة",
                    StartFormNumber = 200
                }
            );

        }
    }
}
