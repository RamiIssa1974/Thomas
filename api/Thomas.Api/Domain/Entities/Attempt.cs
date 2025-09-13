namespace Thomas.Api.Domain.Entities;

public enum AttemptMode { Practice = 0, Real = 1 }
public enum AttemptStatus { NotStarted, InProgress, Completed, Cancelled, Expired }

public class Attempt
{
    public long Id { get; set; }

    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public AttemptMode Mode { get; set; } = AttemptMode.Practice;
    public AttemptStatus Status { get; set; } = AttemptStatus.NotStarted;

    // consents
    public bool ConsentDataPrivacy { get; set; }
    public bool ConsentNoCheating { get; set; }
    public bool ConsentTimeLimits { get; set; }
    public bool ConsentResultsUsage { get; set; }

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? TotalTimeSeconds { get; set; }

    public string? ClientUserAgent { get; set; }
    public string? ClientIp { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AttemptSection> Sections { get; set; } = new List<AttemptSection>();
    public ICollection<AttemptAnswer> Answers { get; set; } = new List<AttemptAnswer>();
    public ICollection<AttemptQuestion> AttemptQuestions { get; set; } = new List<AttemptQuestion>();
    public Report? Report { get; set; }
}
