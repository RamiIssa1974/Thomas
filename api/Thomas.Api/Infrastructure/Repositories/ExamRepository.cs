using Microsoft.EntityFrameworkCore;
using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Infrastructure.Repositories;

public class ExamRepository : IExamRepository
{
    private readonly ThomasDbContext _db;
    public ExamRepository(ThomasDbContext db) => _db = db;

    public Task<List<Exam>> GetActiveAsync(CancellationToken ct = default)
        => _db.Exams
              .AsNoTracking()
              .Where(e => e.IsActive)
              .OrderBy(e => e.Id)
              .ToListAsync(ct);

    public Task<Exam?> GetByCodeWithSectionsAsync(string code, CancellationToken ct = default)
        => _db.Exams
              .AsNoTracking()
              .Where(e => e.Code == code)
              .Select(e => new Exam
              {
                  Id = e.Id,
                  Code = e.Code,
                  Title = e.Title,
                  Description = e.Description,
                  IsActive = e.IsActive,
                  CreatedAt = e.CreatedAt,
                  Sections = e.Sections
                      .Where(s => s.IsEnabled)
                      .OrderBy(s => s.OrderIndex)
                      .Select(s => new ExamSection
                      {
                          Id = s.Id,
                          ExamId = s.ExamId,
                          Name = s.Name,
                          Description = s.Description,
                          TimeLimitSeconds = s.TimeLimitSeconds,
                          OrderIndex = s.OrderIndex,
                          IsEnabled = s.IsEnabled,
                          RealQuestionCount = s.RealQuestionCount
                      }).ToList()
              })
              .FirstOrDefaultAsync(ct);
}
