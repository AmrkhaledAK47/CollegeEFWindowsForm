using Collage.WF.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Collage.WF.Data.Configurations
{
    public class CourseSessionAttendanceConfiguration : IEntityTypeConfiguration<CourseSessionAttendance>
    {
        public void Configure(EntityTypeBuilder<CourseSessionAttendance> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Notes).HasColumnType("varchar(max)");

            builder.HasOne(a => a.CourseSession)
                .WithMany(cs => cs.Attendances)
                .HasForeignKey(a => a.CourseSessionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
