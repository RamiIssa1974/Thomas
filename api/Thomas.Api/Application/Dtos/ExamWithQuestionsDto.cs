namespace Thomas.Api.Application.Dtos;

public class ExamWithQuestionsDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public List<SectionWithQuestionsDto> Sections { get; set; } = new();
}


