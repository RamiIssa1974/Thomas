using System.ComponentModel.DataAnnotations;

namespace Thomas.Api.Domain.Entities;

public enum QuestionType { SingleChoice = 0, MultipleChoice = 1, Numeric = 2 }

public class Question
{
    public int Id { get; set; }
    public int ExamSectionId { get; set; }
    public ExamSection ExamSection { get; set; } = null!;

    public string Stem { get; set; } = null!;
    public QuestionType Type { get; set; } = QuestionType.SingleChoice;

    public int? Difficulty { get; set; }
    public int OrderIndex { get; set; } = 0;

    public bool IsPractice { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}
