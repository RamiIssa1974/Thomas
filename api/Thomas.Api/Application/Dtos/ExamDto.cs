namespace Thomas.Api.Application.Dtos;

public class ExamDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ExamSectionDto> Sections { get; set; } = new();
}
