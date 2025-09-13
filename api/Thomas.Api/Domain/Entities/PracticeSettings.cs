namespace Thomas.Api.Domain.Entities;

public class PracticeSettings
{
    public int Id { get; set; }

    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int TotalQuestions { get; set; } = 15;      // 5 sections * 3
    public int? TimeLimitSeconds { get; set; } = null; // no limit
    public bool InstantFeedback { get; set; } = true;
    public bool ShuffleQuestions { get; set; } = true;
    public bool ShuffleOptions { get; set; } = true;
    public int? PerSectionCount { get; set; } = 3;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
