using TLighthouse.Domain.Constants;
using TLighthouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TLighthouse.Infrastructure.Persistence.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.FullName).IsRequired();
        builder.HasIndex(s => s.Phone).HasPrefixLength(Sql.PhonePrefix);
        builder.HasIndex(s => s.Email).HasPrefixLength(Sql.EmailPrefix);
    }
}

internal sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments", t => t.HasCheckConstraint("ck_enrollments_status", Sql.In("status", EnrollmentStatuses.All)));
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).IsRequired();
        builder.HasOne(e => e.Student).WithMany(s => s.Enrollments).HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Classroom).WithMany(c => c.Enrollments).HasForeignKey(e => e.ClassId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => new { e.ClassId, e.Status }).HasPrefixLength(0, Sql.StatusPrefix);
        builder.HasIndex(e => e.Status).HasPrefixLength(Sql.StatusPrefix);
    }
}

internal sealed class CurriculumProgramConfiguration : IEntityTypeConfiguration<CurriculumProgram>
{
    public void Configure(EntityTypeBuilder<CurriculumProgram> builder)
    {
        builder.ToTable("curriculum_programs");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Code).IsRequired();
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Level).IsRequired();
        builder.Property(p => p.LessonsPerUnit).HasDefaultValue(7);
        builder.Property(p => p.SessionsPerWeek).HasDefaultValue(2);
        builder.HasIndex(p => p.Code).IsUnique().HasPrefixLength(Sql.CodePrefix);
        builder.HasIndex(p => p.FeePlanId);
        builder.HasMany(p => p.FeePlans).WithOne(f => f.Program).HasForeignKey(f => f.ProgramId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ClassroomConfiguration : IEntityTypeConfiguration<Classroom>
{
    public void Configure(EntityTypeBuilder<Classroom> builder)
    {
        builder.ToTable("classes", t => t.HasCheckConstraint("ck_classes_status", Sql.In("status", ClassStatuses.All)));
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).IsRequired();
        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.Status).IsRequired();
        builder.Property(c => c.Capacity).HasDefaultValue(16);
        builder.HasIndex(c => c.Code).IsUnique().HasPrefixLength(Sql.CodePrefix);
        builder.HasIndex(c => c.Status).HasPrefixLength(Sql.StatusPrefix);
        builder.HasOne(c => c.Program).WithMany(p => p.Classes).HasForeignKey(c => c.ProgramId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions", t => t.HasCheckConstraint("ck_sessions_session_type", Sql.In("session_type", SessionTypes.All)));
        builder.HasKey(s => s.Id);
        builder.Ignore(s => s.EffectiveTeacherId);
        builder.Property(s => s.SessionType).IsRequired();
        builder.HasOne(s => s.Classroom).WithMany(c => c.Sessions).HasForeignKey(s => s.ClassId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.MainTeacher).WithMany().HasForeignKey(s => s.MainTeacherId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.CoverTeacher).WithMany().HasForeignKey(s => s.CoverTeacherId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(s => new { s.ClassId, s.SessionDate });
        builder.HasIndex(s => s.SessionDate);
    }
}

internal sealed class SessionFeedbackConfiguration : IEntityTypeConfiguration<SessionFeedback>
{
    public void Configure(EntityTypeBuilder<SessionFeedback> builder)
    {
        builder.ToTable("session_feedback");
        builder.HasKey(f => f.Id);
        builder.HasIndex(f => new { f.SessionId, f.StudentId }).IsUnique();
        builder.HasOne(f => f.Session).WithMany(s => s.Feedbacks).HasForeignKey(f => f.SessionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(f => f.Student).WithMany().HasForeignKey(f => f.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(f => f.Teacher).WithMany().HasForeignKey(f => f.TeacherId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class SessionScoreConfiguration : IEntityTypeConfiguration<SessionScore>
{
    public void Configure(EntityTypeBuilder<SessionScore> builder)
    {
        builder.ToTable("session_scores", t =>
        {
            t.HasCheckConstraint("ck_session_scores_score_type", Sql.In("score_type", SessionTypes.Assessed));
            t.HasCheckConstraint("ck_session_scores_language", Sql.Between("language", 0, 100));
            t.HasCheckConstraint("ck_session_scores_cognitive", Sql.Between("cognitive", 0, 100));
            t.HasCheckConstraint("ck_session_scores_physical", Sql.Between("physical", 0, 100));
            t.HasCheckConstraint("ck_session_scores_social", Sql.Between("social", 0, 100));
            t.HasCheckConstraint("ck_session_scores_total", Sql.Between("total", 0, 100));
        });
        builder.HasKey(s => s.Id);
        builder.Property(s => s.ScoreType).IsRequired();
        builder.HasIndex(s => new { s.SessionId, s.StudentId }).IsUnique();
        builder.HasOne(s => s.Session).WithMany(x => x.Scores).HasForeignKey(s => s.SessionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.Student).WithMany().HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.Restrict);
    }
}
