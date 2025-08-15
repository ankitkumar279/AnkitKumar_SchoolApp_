using Microsoft.EntityFrameworkCore;

namespace AnkitKumar_SchoolApp.Models
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Teachers
            modelBuilder.Entity<Teacher>().ToTable("teachers");
            modelBuilder.Entity<Teacher>().HasKey(t => t.TeacherId);
            modelBuilder.Entity<Teacher>().Property(t => t.TeacherId).HasColumnName("teacherid");
            modelBuilder.Entity<Teacher>().Property(t => t.TeacherFName).HasColumnName("teacherfname");
            modelBuilder.Entity<Teacher>().Property(t => t.TeacherLName).HasColumnName("teacherlname");
            modelBuilder.Entity<Teacher>().Property(t => t.EmployeeNumber).HasColumnName("employeenumber");
            modelBuilder.Entity<Teacher>().Property(t => t.HireDate).HasColumnName("hiredate");
            modelBuilder.Entity<Teacher>().Property(t => t.Salary).HasColumnName("salary");

            // Students
            modelBuilder.Entity<Student>().ToTable("students");
            modelBuilder.Entity<Student>().HasKey(s => s.StudentId);
            modelBuilder.Entity<Student>().Property(s => s.StudentId).HasColumnName("studentid");
            modelBuilder.Entity<Student>().Property(s => s.StudentFName).HasColumnName("studentfname");
            modelBuilder.Entity<Student>().Property(s => s.StudentLName).HasColumnName("studentlname");
            modelBuilder.Entity<Student>().Property(s => s.StudentNumber).HasColumnName("studentnumber");
            modelBuilder.Entity<Student>().Property(s => s.EnrolDate).HasColumnName("enroldate");

            // Courses
            modelBuilder.Entity<Course>().ToTable("courses");
            modelBuilder.Entity<Course>().HasKey(c => c.CourseId);
            modelBuilder.Entity<Course>().Property(c => c.CourseId).HasColumnName("courseid");
            modelBuilder.Entity<Course>().Property(c => c.CourseCode).HasColumnName("coursecode");
            modelBuilder.Entity<Course>().Property(c => c.TeacherId).HasColumnName("teacherid");
            modelBuilder.Entity<Course>().Property(c => c.StartDate).HasColumnName("startdate");
            modelBuilder.Entity<Course>().Property(c => c.FinishDate).HasColumnName("finishdate");
            modelBuilder.Entity<Course>().Property(c => c.CourseName).HasColumnName("coursename");

            // StudentCourse
            modelBuilder.Entity<StudentCourse>().ToTable("studentsxcourses"); // table name
            modelBuilder.Entity<StudentCourse>().HasKey(sc => sc.StudentXCourseId);
            modelBuilder.Entity<StudentCourse>().Property(sc => sc.StudentXCourseId).HasColumnName("studentxcoursid");
            modelBuilder.Entity<StudentCourse>().Property(sc => sc.StudentId).HasColumnName("studentid");
            modelBuilder.Entity<StudentCourse>().Property(sc => sc.CourseId).HasColumnName("courseid");
           

        }
    }
}
