using Thomas.Api.Application.Dtos;
using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Mapping;

public static class ExamQuestionMapping
{
    // Candidate/practice-safe (no IsCorrect)
    public static ExamWithQuestionsDto ToPracticeDto(this Exam e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Title = e.Title,
        Description = e.Description,
        Sections = e.Sections
            .OrderBy(s => s.OrderIndex)
            .Select(s => new SectionWithQuestionsDto
            {
                Id = s.Id,
                Name = s.Name,
                OrderIndex = s.OrderIndex,
                IsEnabled = s.IsEnabled,
                Questions = s.Questions
                    .Where(q => q.IsPractice && q.IsActive)
                    .OrderBy(q => q.OrderIndex)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Stem = q.Stem,
                        Type = q.Type,
                        OrderIndex = q.OrderIndex,
                        IsPractice = q.IsPractice,
                        Options = q.Options
                            .OrderBy(o => o.OrderIndex)
                            .Select(o => new QuestionOptionDto
                            {
                                Id = o.Id,
                                Text = o.Text,
                                OrderIndex = o.OrderIndex,
                                IsCorrect = null // hide correct flags for candidates
                            }).ToList()
                    }).ToList()
            }).ToList()
    };

    // Admin view (includes IsCorrect on options)
    public static ExamWithQuestionsDto ToAdminDto(this Exam e) => new()
    {
        Id = e.Id,
        Code = e.Code,
        Title = e.Title,
        Description = e.Description,
        Sections = e.Sections
            .OrderBy(s => s.OrderIndex)
            .Select(s => new SectionWithQuestionsDto
            {
                Id = s.Id,
                Name = s.Name,
                OrderIndex = s.OrderIndex,
                IsEnabled = s.IsEnabled,
                Questions = s.Questions
                    .Where(q => q.IsActive)
                    .OrderBy(q => q.OrderIndex)
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Stem = q.Stem,
                        Type = q.Type,
                        OrderIndex = q.OrderIndex,
                        IsPractice = q.IsPractice,
                        Options = q.Options
                            .OrderBy(o => o.OrderIndex)
                            .Select(o => new QuestionOptionDto
                            {
                                Id = o.Id,
                                Text = o.Text,
                                OrderIndex = o.OrderIndex,
                                IsCorrect = o.IsCorrect
                            }).ToList()
                    }).ToList()
            }).ToList()
    };
}
