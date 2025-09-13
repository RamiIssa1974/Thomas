using Thomas.Api.Application.Dtos;

namespace Thomas.Api.Application.Interfaces;

public interface IAttemptService
{

    Task StartSectionAsync(long attemptId, int sectionId, CancellationToken ct);

    Task<CreateAttemptResponse> CreateAsync(CreateAttemptRequest req, Guid userId, CancellationToken ct);
    Task<NextQuestionDto?> GetNextQuestionAsync(long attemptId, int examSectionId, CancellationToken ct);
    Task<SubmitAnswerResult> SubmitAnswerAsync(long attemptId, AttemptModeDto mode, SubmitAnswerRequest req, CancellationToken ct);
    Task<CompleteAttemptResponse> CompleteAsync(long attemptId, CancellationToken ct);
}
