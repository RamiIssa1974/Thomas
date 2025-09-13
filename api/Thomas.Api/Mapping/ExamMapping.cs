using Thomas.Api.Application.Dtos;
using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Mapping;

public static class ExamMapping
{
    public static ExamDto ToDto(this Exam e) =>
        new()
        {
            Id = e.Id,
            Code = e.Code,
            Title = e.Title,
            Description = e.Description,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            Sections = e.Sections?
                .OrderBy(s => s.OrderIndex)
                .Select(s => new ExamSectionDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    TimeLimitSeconds = s.TimeLimitSeconds,
                    OrderIndex = s.OrderIndex,
                    IsEnabled = s.IsEnabled,
                    RealQuestionCount = s.RealQuestionCount
                }).ToList() ?? new()
        };

    public static List<ExamDto> ToDto(this IEnumerable<Exam> exams) => exams.Select(ToDto).ToList();
}
