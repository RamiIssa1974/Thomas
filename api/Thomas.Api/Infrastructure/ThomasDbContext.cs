using Microsoft.EntityFrameworkCore;
using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Infrastructure;

public class ThomasDbContext(DbContextOptions<ThomasDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamSection> ExamSections => Set<ExamSection>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<Attempt> Attempts => Set<Attempt>();
    public DbSet<AttemptSection> AttemptSections => Set<AttemptSection>();
    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<PracticeSettings> PracticeSettings => Set<PracticeSettings>();
    public DbSet<AttemptQuestion> AttemptQuestions => Set<AttemptQuestion>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Users
        b.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();
        b.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>()       // "Admin" / "Candidate"
            .HasMaxLength(50);

        // Exams
        b.Entity<Exam>()
            .HasIndex(e => e.Code).IsUnique();

        // Sections
        b.Entity<ExamSection>()
            .HasOne(s => s.Exam).WithMany(e => e.Sections)
            .HasForeignKey(s => s.ExamId).OnDelete(DeleteBehavior.Cascade);

        // Questions
        b.Entity<Question>()
            .HasOne(q => q.ExamSection).WithMany(s => s.Questions)
            .HasForeignKey(q => q.ExamSectionId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Question>()
            .Property(q => q.Type)
            .HasConversion<string>()       // "SingleChoice" / "MultipleChoice" / "Numeric"
            .HasMaxLength(50);

        // Options
        b.Entity<QuestionOption>()
            .HasOne(o => o.Question).WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId).OnDelete(DeleteBehavior.Cascade);

        // Attempt
        b.Entity<Attempt>()
            .HasOne(a => a.Exam).WithMany().HasForeignKey(a => a.ExamId);
        b.Entity<Attempt>()
            .HasOne(a => a.User).WithMany(u => u.Attempts).HasForeignKey(a => a.UserId);
        b.Entity<Attempt>()
            .Property(a => a.Mode)
            .HasConversion<string>()       // "Practice" / "Real"
            .HasMaxLength(20);
        b.Entity<Attempt>()
            .Property(a => a.Status)
            .HasConversion<string>()       // "NotStarted", etc.
            .HasMaxLength(20);

        // AttemptSection
        b.Entity<AttemptSection>()
            .HasIndex(x => new { x.AttemptId, x.ExamSectionId }).IsUnique();
        b.Entity<AttemptSection>()
            .HasOne(s => s.Attempt).WithMany(a => a.Sections)
            .HasForeignKey(s => s.AttemptId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<AttemptSection>()
            .HasOne(s => s.ExamSection).WithMany()
            .HasForeignKey(s => s.ExamSectionId);
        b.Entity<AttemptSection>()
            .Property(s => s.Status)
            .HasConversion<string>()       // "NotStarted", etc.
            .HasMaxLength(20);

        // AttemptAnswer
        b.Entity<AttemptAnswer>()
            .HasIndex(x => new { x.AttemptId, x.QuestionId }).IsUnique();
        b.Entity<AttemptAnswer>()
            .Property(x => x.SelectedOptionIds).HasColumnType("nvarchar(max)");
        b.Entity<AttemptAnswer>()
            .HasOne(a => a.Attempt).WithMany(p => p.Answers)
            .HasForeignKey(a => a.AttemptId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<AttemptAnswer>()
            .HasOne(a => a.ExamSection).WithMany().HasForeignKey(a => a.ExamSectionId);
        b.Entity<AttemptAnswer>()
            .HasOne(a => a.Question).WithMany().HasForeignKey(a => a.QuestionId);

        // Report 1:1
        b.Entity<Report>()
            .HasIndex(r => r.AttemptId).IsUnique();
        b.Entity<Report>()
            .HasOne(r => r.Attempt).WithOne(a => a.Report)
            .HasForeignKey<Report>(r => r.AttemptId).OnDelete(DeleteBehavior.Cascade);

        // PracticeSettings 1:1
        b.Entity<PracticeSettings>()
            .HasIndex(p => p.ExamId).IsUnique();
        b.Entity<PracticeSettings>()
            .HasOne(p => p.Exam).WithOne(e => e.PracticeSettings)
            .HasForeignKey<PracticeSettings>(p => p.ExamId).OnDelete(DeleteBehavior.Cascade);

        // AttemptQuestion
        b.Entity<AttemptQuestion>()
            .HasIndex(x => new { x.AttemptId, x.QuestionId }).IsUnique();
        b.Entity<AttemptQuestion>()
            .HasIndex(x => new { x.AttemptId, x.ExamSectionId, x.SequenceIndex }).IsUnique();
        b.Entity<AttemptQuestion>()
            .HasOne(x => x.Attempt).WithMany(a => a.AttemptQuestions)
            .HasForeignKey(x => x.AttemptId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<AttemptQuestion>()
            .HasOne(x => x.ExamSection).WithMany().HasForeignKey(x => x.ExamSectionId);
        b.Entity<AttemptQuestion>()
            .HasOne(x => x.Question).WithMany().HasForeignKey(x => x.QuestionId);

        base.OnModelCreating(b);
    }

}
