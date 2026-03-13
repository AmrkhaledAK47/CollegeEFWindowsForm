using Collage.WF.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Collage.WF.Data.Configurations
{
    public class CourseSessionConfiguration : IEntityTypeConfiguration<CourseSession>
    {
        public void Configure(EntityTypeBuilder<CourseSession> builder)
        {
            builder.HasKey(cs => cs.Id);

            builder.Property(cs => cs.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(cs => cs.Course)
                .WithMany(c => c.CourseSessions)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(cs => cs.Instructor)
                .WithMany(i => i.CourseSessions)
                .HasForeignKey(cs => cs.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
