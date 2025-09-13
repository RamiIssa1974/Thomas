using Thomas.Api.Application.Dtos;
using Thomas.Api.Application.Interfaces;
using Thomas.Api.Infrastructure.Repositories;
using Thomas.Api.Mapping;

namespace Thomas.Api.Application.Services;

public class ExamService : IExamService
{
    private readonly IExamRepository _repo;
    public ExamService(IExamRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<ExamDto>> GetActiveAsync(CancellationToken ct = default)
    {
        var models = await _repo.GetActiveAsync(ct);
        return models.ToDto();
    }

    public async Task<ExamDto?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var model = await _repo.GetByCodeWithSectionsAsync(code, ct);
        return model?.ToDto();
    }
}
