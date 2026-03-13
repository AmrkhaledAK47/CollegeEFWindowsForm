namespace Collage.WF.Data.Models
{
    public class Department
    {
        public int Id { get; set; }
        public int? ManagerId { get; set; }
        public string Name { get; set; } = null!;
        public string? Location { get; set; }

        public virtual Instructor? Manager { get; set; }
        public virtual ICollection<Instructor> Instructors { get; set; } = new HashSet<Instructor>();
        public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    }
}
