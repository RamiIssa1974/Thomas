using Microsoft.EntityFrameworkCore;
using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Infrastructure.Repositories;

public class ExamBankRepository : IExamBankRepository
{
    private readonly ThomasDbContext _db;
    public ExamBankRepository(ThomasDbContext db) => _db = db;

    // Candidate-safe: only practice, no answer keys returned by mapper
    public Task<Exam?> GetExamPracticeBankAsync(string code, CancellationToken ct = default)
        => _db.Exams
            .AsNoTracking()
            .Where(e => e.Code == code && e.IsActive)
            .Select(e => new Exam
            {
                Id = e.Id,
                Code = e.Code,
                Title = e.Title,
                Description = e.Description,
                Sections = e.Sections
                    .Where(s => s.IsEnabled)
                    .Select(s => new ExamSection
                    {
                        Id = s.Id,
                        ExamId = s.ExamId,
                        Name = s.Name,
                        Description = s.Description,
                        TimeLimitSeconds = s.TimeLimitSeconds,
                        OrderIndex = s.OrderIndex,
                        IsEnabled = s.IsEnabled,
                        RealQuestionCount = s.RealQuestionCount,
                        Questions = s.Questions
                            .Where(q => q.IsPractice && q.IsActive)
                            .Select(q => new Question
                            {
                                Id = q.Id,
                                ExamSectionId = q.ExamSectionId,
                                Stem = q.Stem,
                                Type = q.Type,
                                OrderIndex = q.OrderIndex,
                                IsPractice = q.IsPractice,
                                IsActive = q.IsActive,
                                Options = q.Options
                                    .Select(o => new QuestionOption
                                    {
                                        Id = o.Id,
                                        QuestionId = o.QuestionId,
                                        Text = o.Text,
                                        IsCorrect = o.IsCorrect,
                                        OrderIndex = o.OrderIndex,
                                        Value = o.Value
                                    }).ToList()
                            }).ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync(ct);

    // Admin: full bank (practice + real)
    public Task<Exam?> GetExamFullBankAsync(string code, CancellationToken ct = default)
        => _db.Exams
            .AsNoTracking()
            .Where(e => e.Code == code)
            .Select(e => new Exam
            {
                Id = e.Id,
                Code = e.Code,
                Title = e.Title,
                Description = e.Description,
                Sections = e.Sections
                    .Select(s => new ExamSection
                    {
                        Id = s.Id,
                        ExamId = s.ExamId,
                        Name = s.Name,
                        Description = s.Description,
                        TimeLimitSeconds = s.TimeLimitSeconds,
                        OrderIndex = s.OrderIndex,
                        IsEnabled = s.IsEnabled,
                        RealQuestionCount = s.RealQuestionCount,
                        Questions = s.Questions
                            .Where(q => q.IsActive)
                            .Select(q => new Question
                            {
                                Id = q.Id,
                                ExamSectionId = q.ExamSectionId,
                                Stem = q.Stem,
                                Type = q.Type,
                                OrderIndex = q.OrderIndex,
                                IsPractice = q.IsPractice,
                                IsActive = q.IsActive,
                                Options = q.Options
                                    .Select(o => new QuestionOption
                                    {
                                        Id = o.Id,
                                        QuestionId = o.QuestionId,
                                        Text = o.Text,
                                        IsCorrect = o.IsCorrect,
                                        OrderIndex = o.OrderIndex,
                                        Value = o.Value
                                    }).ToList()
                            }).ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync(ct);
}
