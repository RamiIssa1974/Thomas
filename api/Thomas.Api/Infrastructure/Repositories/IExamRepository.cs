using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Infrastructure.Repositories;

public interface IExamRepository
{
    Task<List<Exam>> GetActiveAsync(CancellationToken ct = default);
    Task<Exam?> GetByCodeWithSectionsAsync(string code, CancellationToken ct = default);
}
