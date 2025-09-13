using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Application.Dtos;

public class QuestionDto
{
    public int Id { get; set; }
    public string Stem { get; set; } = "";
    public QuestionType Type { get; set; } = QuestionType.SingleChoice;
    public int OrderIndex { get; set; }
    public bool IsPractice { get; set; }
    public List<QuestionOptionDto> Options { get; set; } = new();
}
