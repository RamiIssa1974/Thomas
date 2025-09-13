namespace Thomas.Api.Domain.Entities;

public class AttemptAnswer
{
    public long Id { get; set; }

    public long AttemptId { get; set; }
    public Attempt Attempt { get; set; } = null!;

    public int ExamSectionId { get; set; }
    public ExamSection ExamSection { get; set; } = null!;

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    // JSON like: [9001,9002]
    public string? SelectedOptionIds { get; set; }

    public decimal? NumericValue { get; set; }
    public bool? IsCorrect { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    public int? TimeToAnswerMs { get; set; }
}
