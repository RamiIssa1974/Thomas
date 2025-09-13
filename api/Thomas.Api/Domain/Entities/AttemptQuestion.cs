namespace Thomas.Api.Domain.Entities;

public class AttemptQuestion
{
    public long Id { get; set; }

    public long AttemptId { get; set; }
    public Attempt Attempt { get; set; } = null!;

    public int ExamSectionId { get; set; }
    public ExamSection ExamSection { get; set; } = null!;

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int SequenceIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
