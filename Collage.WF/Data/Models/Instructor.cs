namespace Collage.WF.Data.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
        public virtual ICollection<CourseSession> CourseSessions { get; set; } = new HashSet<CourseSession>();
        public virtual Department? ManagedDepartment { get; set; }
    }
}
