using System.ComponentModel.DataAnnotations;

namespace Thomas.Api.Domain.Entities;

public class Exam
{
    public int Id { get; set; }

    [MaxLength(50)] public string Code { get; set; } = null!;
    [MaxLength(200)] public string Title { get; set; } = null!;
    [MaxLength(1000)] public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ExamSection> Sections { get; set; } = new List<ExamSection>();
    public PracticeSettings? PracticeSettings { get; set; }
}
