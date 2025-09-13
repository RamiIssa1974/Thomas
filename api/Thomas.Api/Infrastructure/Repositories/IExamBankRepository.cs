using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Infrastructure.Repositories;

public interface IExamBankRepository
{
    Task<Exam?> GetExamPracticeBankAsync(string code, CancellationToken ct = default);
    Task<Exam?> GetExamFullBankAsync(string code, CancellationToken ct = default);
}
