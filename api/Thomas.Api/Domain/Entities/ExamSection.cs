using System.ComponentModel.DataAnnotations;

namespace Thomas.Api.Domain.Entities;

public class ExamSection
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    [MaxLength(200)] public string Name { get; set; } = null!;
    [MaxLength(1000)] public string? Description { get; set; }

    public int TimeLimitSeconds { get; set; } = 0;
    public int OrderIndex { get; set; } = 0;
    public bool IsEnabled { get; set; } = true;

    // real exam target per section (keep it simple)
    public int RealQuestionCount { get; set; } = 20;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
