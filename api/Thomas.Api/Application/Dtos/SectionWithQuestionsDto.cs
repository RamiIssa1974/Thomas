namespace Thomas.Api.Application.Dtos
{
    public class SectionWithQuestionsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int OrderIndex { get; set; }
        public bool IsEnabled { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }
}
