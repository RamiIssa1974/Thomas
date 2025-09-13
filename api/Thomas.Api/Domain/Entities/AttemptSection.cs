namespace Thomas.Api.Domain.Entities;

public enum SectionStatus { NotStarted, InProgress, Completed, Expired }

public class AttemptSection
{
    public long Id { get; set; }

    public long AttemptId { get; set; }
    public Attempt Attempt { get; set; } = null!;

    public int ExamSectionId { get; set; }
    public ExamSection ExamSection { get; set; } = null!;

    public SectionStatus Status { get; set; } = SectionStatus.NotStarted;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public int? TimeSpentSeconds { get; set; }
    public int RawScore { get; set; } = 0;
    public int MaxScore { get; set; } = 0;
}
