using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Infrastructure.Repositories;

public interface IAttemptRepository
{
    // IAttemptRepository
    Task CompleteSectionIfDoneAsync(long attemptId, int sectionId, CancellationToken ct);

    // IAttemptRepository
    Task<bool> QuestionBelongsToAttemptAsync(long attemptId, int sectionId, int questionId, CancellationToken ct);
    Task<bool> HasAnsweredAsync(long attemptId, int questionId, CancellationToken ct);

    // IAttemptRepository
    Task StartSectionAsync(long attemptId, int sectionId, CancellationToken ct);

    Task<Exam?> GetActiveExamByCodeAsync(string code, CancellationToken ct);
    Task<long> CreateAttemptAsync(Attempt attempt, IEnumerable<AttemptSection> sections, CancellationToken ct);

    Task<bool> AttemptExistsAsync(long attemptId, CancellationToken ct);
    Task<Attempt?> GetAttemptWithExamAsync(long attemptId, CancellationToken ct);

    Task<List<int>> GetPracticeQuestionIdsPerSectionAsync(int examSectionId, int take, CancellationToken ct);
    Task<List<int>> GetRealQuestionIdsPerSectionAsync(int examSectionId, int take, CancellationToken ct);

    Task AddAttemptQuestionsAsync(long attemptId, int examSectionId, IReadOnlyList<int> questionIds, CancellationToken ct);
    Task<(int? questionId, int sequenceIndex)?> GetNextUnansweredAsync(long attemptId, int examSectionId, CancellationToken ct);
    Task<Question?> GetQuestionWithOptionsAsync(int questionId, CancellationToken ct);

    Task SaveAnswerAsync(AttemptAnswer answer, CancellationToken ct);

    Task<(int raw, int max)> ComputeSectionScoresAsync(long attemptId, int examSectionId, CancellationToken ct);
    Task<List<(int sectionId, string sectionName, int raw, int max)>> ComputeAllSectionsAsync(long attemptId, CancellationToken ct);

    Task SaveReportAsync(long attemptId, string json, CancellationToken ct);
    Task MarkAttemptCompletedAsync(long attemptId, CancellationToken ct);
}
