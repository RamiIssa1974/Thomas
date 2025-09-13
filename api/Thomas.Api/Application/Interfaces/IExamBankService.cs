using Thomas.Api.Application.Dtos;

namespace Thomas.Api.Application.Interfaces;

public interface IExamBankService
{
    Task<ExamWithQuestionsDto?> GetPracticeBankAsync(string examCode, CancellationToken ct = default);
    Task<ExamWithQuestionsDto?> GetFullBankAdminAsync(string examCode, CancellationToken ct = default);
}
