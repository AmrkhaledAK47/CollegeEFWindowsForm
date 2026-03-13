namespace Collage.WF.Data.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }

        public virtual ICollection<CourseStudent> CourseStudents { get; set; } = new HashSet<CourseStudent>();
        public virtual ICollection<CourseSessionAttendance> Attendances { get; set; } = new HashSet<CourseSessionAttendance>();
    }
}
