namespace Thomas.Api.Application.Dtos;

public class ExamSectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int TimeLimitSeconds { get; set; }
    public int OrderIndex { get; set; }
    public bool IsEnabled { get; set; }
    public int RealQuestionCount { get; set; }
}
