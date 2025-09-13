using Thomas.Api.Application.Dtos;

namespace Thomas.Api.Application.Interfaces;

public interface IExamService
{
    Task<IReadOnlyList<ExamDto>> GetActiveAsync(CancellationToken ct = default);
    Task<ExamDto?> GetByCodeAsync(string code, CancellationToken ct = default);
}
