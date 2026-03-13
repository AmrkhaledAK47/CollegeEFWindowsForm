namespace Collage.WF.Data.Models
{
    public class Course
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int InstructorId { get; set; }
        public int? Duration { get; set; }
        public string? Name { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual Instructor Instructor { get; set; } = null!;
        public virtual ICollection<CourseSession> CourseSessions { get; set; } = new HashSet<CourseSession>();
        public virtual ICollection<CourseStudent> CourseStudents { get; set; } = new HashSet<CourseStudent>();
    }
}
