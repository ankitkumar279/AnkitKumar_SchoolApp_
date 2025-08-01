using Microsoft.EntityFrameworkCore;

namespace AnkitKumar_SchoolApp.Models
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

        public DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>().ToTable("teachers");
            modelBuilder.Entity<Teacher>().HasKey(t => t.TeacherId);

            modelBuilder.Entity<Teacher>()
                .Property(t => t.TeacherId)
                .HasColumnName("teacherid");

            modelBuilder.Entity<Teacher>()
                .Property(t => t.TeacherFName)
                .HasColumnName("teacherfname");

            modelBuilder.Entity<Teacher>()
                .Property(t => t.TeacherLName)
                .HasColumnName("teacherlname");

            modelBuilder.Entity<Teacher>()
                .Property(t => t.EmployeeNumber)
                .HasColumnName("employeenumber");

            modelBuilder.Entity<Teacher>()
                .Property(t => t.HireDate)
                .HasColumnName("hiredate");

            modelBuilder.Entity<Teacher>()
                .Property(t => t.Salary)
                .HasColumnName("salary");
        }
    }
}
