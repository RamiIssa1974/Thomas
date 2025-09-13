using System.ComponentModel.DataAnnotations;

namespace Thomas.Api.Domain.Entities;

public class QuestionOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public string Text { get; set; } = null!;
    public bool IsCorrect { get; set; } = false;
    public int OrderIndex { get; set; } = 0;

    [MaxLength(100)] public string? Value { get; set; }
}
