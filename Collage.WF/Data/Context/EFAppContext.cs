using Collage.WF.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Collage.WF
{
    public class EFAppContext : DbContext
    {
        public EFAppContext() { }
        public EFAppContext(DbContextOptions<EFAppContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSession> CourseSessions { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; }
        public DbSet<CourseSessionAttendance> CourseSessionAttendances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=.\\SQLEXPRESS;DataBase=CollegeEF;Trusted_Connection=true;TrustServerCertificate=true";
            optionsBuilder.
                UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EFAppContext).Assembly);

            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Computer Science", Location = "Building A", ManagerId = null },
                new Department { Id = 2, Name = "Mathematics", Location = "Building B", ManagerId = null },
                new Department { Id = 3, Name = "Physics", Location = "Building C", ManagerId = null }
            );

            modelBuilder.Entity<Instructor>().HasData(
                new Instructor { Id = 1, DepartmentId = 1, FirstName = "Ahmed", LastName = "Ali", Phone = "0100000001" },
                new Instructor { Id = 2, DepartmentId = 1, FirstName = "Sara", LastName = "Hassan", Phone = "0100000002" },
                new Instructor { Id = 3, DepartmentId = 2, FirstName = "Mohamed", LastName = "Nour", Phone = "0100000003" },
                new Instructor { Id = 4, DepartmentId = 3, FirstName = "Fatma", LastName = "Salem", Phone = "0100000004" }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, DepartmentId = 1, InstructorId = 1, Duration = 40, Name = "C# Programming" },
                new Course { Id = 2, DepartmentId = 1, InstructorId = 2, Duration = 30, Name = "Database Systems" },
                new Course { Id = 3, DepartmentId = 2, InstructorId = 3, Duration = 35, Name = "Linear Algebra" },
                new Course { Id = 4, DepartmentId = 3, InstructorId = 4, Duration = 25, Name = "Quantum Mechanics" }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, FirstName = "Omar", LastName = "Khaled", Phone = "0110000001" },
                new Student { Id = 2, FirstName = "Nada", LastName = "Ibrahim", Phone = "0110000002" },
                new Student { Id = 3, FirstName = "Youssef", LastName = "Tarek", Phone = "0110000003" },
                new Student { Id = 4, FirstName = "Mona", LastName = "Adel", Phone = "0110000004" },
                new Student { Id = 5, FirstName = "Ali", LastName = "Mostafa", Phone = "0110000005" }
            );

            modelBuilder.Entity<CourseSession>().HasData(
                new CourseSession { Id = 1, CourseId = 1, InstructorId = 1, Date = new DateTime(2025, 1, 10), Title = "Intro to C#" },
                new CourseSession { Id = 2, CourseId = 1, InstructorId = 1, Date = new DateTime(2025, 1, 17), Title = "OOP Basics" },
                new CourseSession { Id = 3, CourseId = 2, InstructorId = 2, Date = new DateTime(2025, 2, 5), Title = "SQL Fundamentals" },
                new CourseSession { Id = 4, CourseId = 3, InstructorId = 3, Date = new DateTime(2025, 2, 12), Title = "Vectors & Matrices" }
            );

            modelBuilder.Entity<CourseStudent>().HasData(
                new CourseStudent { CourseId = 1, StudentId = 1 },
                new CourseStudent { CourseId = 1, StudentId = 2 },
                new CourseStudent { CourseId = 1, StudentId = 3 },
                new CourseStudent { CourseId = 2, StudentId = 2 },
                new CourseStudent { CourseId = 2, StudentId = 4 },
                new CourseStudent { CourseId = 3, StudentId = 3 },
                new CourseStudent { CourseId = 3, StudentId = 5 },
                new CourseStudent { CourseId = 4, StudentId = 1 },
                new CourseStudent { CourseId = 4, StudentId = 5 }
            );

            modelBuilder.Entity<CourseSessionAttendance>().HasData(
                new CourseSessionAttendance { Id = 1, CourseSessionId = 1, StudentId = 1, Grade = 85, Notes = "Good participation" },
                new CourseSessionAttendance { Id = 2, CourseSessionId = 1, StudentId = 2, Grade = 90, Notes = "Excellent" },
                new CourseSessionAttendance { Id = 3, CourseSessionId = 2, StudentId = 1, Grade = 78, Notes = null },
                new CourseSessionAttendance { Id = 4, CourseSessionId = 2, StudentId = 3, Grade = 92, Notes = "Outstanding" },
                new CourseSessionAttendance { Id = 5, CourseSessionId = 3, StudentId = 2, Grade = 88, Notes = null },
                new CourseSessionAttendance { Id = 6, CourseSessionId = 3, StudentId = 4, Grade = 70, Notes = "Needs improvement" },
                new CourseSessionAttendance { Id = 7, CourseSessionId = 4, StudentId = 3, Grade = 95, Notes = "Top of class" },
                new CourseSessionAttendance { Id = 8, CourseSessionId = 4, StudentId = 5, Grade = 82, Notes = null }
            );

            }
    }
}
