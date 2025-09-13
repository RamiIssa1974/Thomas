namespace Thomas.Api.Application.Dtos;

public class QuestionOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public int OrderIndex { get; set; }

    // Admin-only field; keep null for candidate/practice endpoints
    public bool? IsCorrect { get; set; }
}
