using Thomas.Api.Application.Dtos;
using Thomas.Api.Application.Interfaces;
using Thomas.Api.Infrastructure.Repositories;
using Thomas.Api.Mapping;

namespace Thomas.Api.Application.Services;

public class ExamBankService : IExamBankService
{
    private readonly IExamBankRepository _repo;
    public ExamBankService(IExamBankRepository repo) => _repo = repo;

    public async Task<ExamWithQuestionsDto?> GetPracticeBankAsync(string examCode, CancellationToken ct = default)
    {
        var exam = await _repo.GetExamPracticeBankAsync(examCode, ct);
        return exam?.ToPracticeDto();
    }

    public async Task<ExamWithQuestionsDto?> GetFullBankAdminAsync(string examCode, CancellationToken ct = default)
    {
        var exam = await _repo.GetExamFullBankAsync(examCode, ct);
        return exam?.ToAdminDto();
    }
}
