using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Thomas.Api.Application.Dtos;
using Thomas.Api.Application.Interfaces;
using Thomas.Api.Domain.Entities;
using Thomas.Api.Infrastructure;
using Thomas.Api.Infrastructure.Repositories;

namespace Thomas.Api.Application.Services;

public class AttemptService : IAttemptService
{
    private readonly IAttemptRepository _repo;
    private readonly ThomasDbContext _db;   // <-- inject DbContext

    public AttemptService(IAttemptRepository repo, ThomasDbContext db)
    {
        _repo = repo;
        _db = db;
    }
    // AttemptService.cs
    public Task StartSectionAsync(long attemptId, int sectionId, CancellationToken ct)
        => _repo.StartSectionAsync(attemptId, sectionId, ct);


    public async Task<CreateAttemptResponse> CreateAsync(CreateAttemptRequest req, Guid userId, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var exam = await _repo.GetActiveExamByCodeAsync(req.ExamCode, ct)
                   ?? throw new InvalidOperationException("Exam not found or inactive.");

        var mode = req.Mode == AttemptModeDto.Real ? AttemptMode.Real : AttemptMode.Practice;

        var attempt = new Attempt
        {
            ExamId = exam.Id,
            UserId = userId,
            Mode = mode,
            Status = AttemptStatus.NotStarted,
            ConsentDataPrivacy = req.ConsentDataPrivacy,
            ConsentNoCheating = req.ConsentNoCheating,
            ConsentTimeLimits = req.ConsentTimeLimits,
            ConsentResultsUsage = req.ConsentResultsUsage,
            ClientIp = req.ClientIp,
            ClientUserAgent = req.ClientUserAgent,
            CreatedAt = DateTime.UtcNow
        };

        // Sections snapshot for this attempt
        var sections = exam.Sections
            .Where(s => s.IsEnabled)
            .OrderBy(s => s.OrderIndex)
            .Select(s => new AttemptSection
            {
                ExamSectionId = s.Id,
                Status = SectionStatus.NotStarted,
                RawScore = 0,
                MaxScore = 0
            })
            .ToList();

        var attemptId = await _repo.CreateAttemptAsync(attempt, sections, ct);

        // Prepare fixed question set
        if (mode == AttemptMode.Practice)
        {
            var per = exam.PracticeSettings?.PerSectionCount ?? 3;
            foreach (var s in exam.Sections.Where(s => s.IsEnabled))
            {
                var ids = await _repo.GetPracticeQuestionIdsPerSectionAsync(s.Id, per, ct);
                await _repo.AddAttemptQuestionsAsync(attemptId, s.Id, ids, ct);
            }
        }
        else // Real
        {
            foreach (var s in exam.Sections.Where(s => s.IsEnabled))
            {
                var take = s.RealQuestionCount;
                var ids = await _repo.GetRealQuestionIdsPerSectionAsync(s.Id, take, ct);
                await _repo.AddAttemptQuestionsAsync(attemptId, s.Id, ids, ct);
            }
        }
        await tx.CommitAsync(ct);

        return new CreateAttemptResponse
        {
            AttemptId = attemptId,
            ExamCode = exam.Code,
            Mode = req.Mode,
            CreatedAtUtc = attempt.CreatedAt,
            Sections = sections.Select((s, i) => new AttemptSectionSummaryDto
            {
                ExamSectionId = s.ExamSectionId,
                SectionName = exam.Sections.First(x => x.Id == s.ExamSectionId).Name,
                PlannedQuestions = (mode == AttemptMode.Practice)
                    ? (exam.PracticeSettings?.PerSectionCount ?? 3)
                    : exam.Sections.First(x => x.Id == s.ExamSectionId).RealQuestionCount
            }).ToList()
        };
    }

    public async Task<NextQuestionDto?> GetNextQuestionAsync(long attemptId, int examSectionId, CancellationToken ct)
    {
        var next = await _repo.GetNextUnansweredAsync(attemptId, examSectionId, ct);
        if (next is null) return null;

        var q = await _repo.GetQuestionWithOptionsAsync(next.Value.questionId!.Value, ct)
                ?? throw new InvalidOperationException("Question not found.");

        return new NextQuestionDto
        {
            ExamSectionId = examSectionId,
            QuestionId = q.Id,
            Stem = q.Stem,
            OrderInSection = next.Value.sequenceIndex,
            Type = q.Type.ToString(),
            Options = q.Options
                .OrderBy(o => o.OrderIndex)
                .Select(o => new QuestionOptionLiteDto { Id = o.Id, Text = o.Text, OrderIndex = o.OrderIndex })
                .ToList()
        };
    }

    public async Task<SubmitAnswerResult> SubmitAnswerAsync(long attemptId, AttemptModeDto mode, SubmitAnswerRequest req, CancellationToken ct)
    {
        // אימות שהשאלה שייכת לניסיון ולמקטע
        if (!await _repo.QuestionBelongsToAttemptAsync(attemptId, req.ExamSectionId, req.QuestionId, ct))
            throw new InvalidOperationException("Question does not belong to this attempt/section.");

        // מניעת כפל תשובה
        if (await _repo.HasAnsweredAsync(attemptId, req.QuestionId, ct))
            throw new InvalidOperationException("Question already answered.");

        // load question & correct options
        var q = await _repo.GetQuestionWithOptionsAsync(req.QuestionId, ct)
                ?? throw new InvalidOperationException("Question not found.");

        bool isCorrect;
        List<int>? correctIds = null;

        if (q.Type == QuestionType.SingleChoice || q.Type == QuestionType.MultipleChoice)
        {
            var correct = q.Options.Where(o => o.IsCorrect).Select(o => o.Id).OrderBy(x => x).ToList();
            var chosen = (req.SelectedOptionIds ?? new List<int>()).OrderBy(x => x).ToList();

            isCorrect = correct.SequenceEqual(chosen);
            if (mode == AttemptModeDto.Practice)
                correctIds = correct;
        }
        else
        {
            // numeric: compare exactly for now
            isCorrect = q.Type == QuestionType.Numeric && req.NumericValue is not null
                        && q.Options.Any(o => o.IsCorrect && decimal.TryParse(o.Value, out var v) && v == req.NumericValue.Value);
            if (mode == AttemptModeDto.Practice)
                correctIds = q.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToList();
        }

        var answer = new Domain.Entities.AttemptAnswer
        {
            AttemptId = attemptId,
            ExamSectionId = req.ExamSectionId,
            QuestionId = req.QuestionId,
            SelectedOptionIds = req.SelectedOptionIds is null ? null : JsonSerializer.Serialize(req.SelectedOptionIds),
            NumericValue = req.NumericValue,
            IsCorrect = isCorrect,
            AnsweredAt = DateTime.UtcNow,
            TimeToAnswerMs = req.TimeToAnswerMs
        };
        await _repo.SaveAnswerAsync(answer, ct);
        await _repo.CompleteSectionIfDoneAsync(attemptId, req.ExamSectionId, ct);

        return new SubmitAnswerResult { IsCorrect = isCorrect, CorrectOptionIds = (mode == AttemptModeDto.Practice) ? correctIds : null };
    }

    public async Task<CompleteAttemptResponse> CompleteAsync(long attemptId, CancellationToken ct)
    {
        var attempt = await _repo.GetAttemptWithExamAsync(attemptId, ct)
                      ?? throw new InvalidOperationException("Attempt not found.");
        // compute per-section + overall
        var per = await _repo.ComputeAllSectionsAsync(attemptId, ct);
        var totalRaw = per.Sum(x => x.raw);
        var totalMax = per.Sum(x => x.max);
        var totalAcc = totalMax == 0 ? 0m : Math.Round((decimal)totalRaw * 100m / totalMax, 2);

        var payload = new
        {
            attemptId,
            examCode = attempt.Exam.Code,
            mode = attempt.Mode.ToString(),
            completedAt = DateTime.UtcNow,
            overall = new { score = totalRaw, max = totalMax, accuracy = totalAcc },
            sections = per.Select(x => new
            {
                sectionId = x.sectionId,
                sectionName = x.sectionName,
                score = x.raw,
                max = x.max,
                accuracy = x.max == 0 ? 0 : Math.Round((decimal)x.raw * 100m / x.max, 2)
            }).ToList()
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });
        await _repo.SaveReportAsync(attemptId, json, ct);
        await _repo.MarkAttemptCompletedAsync(attemptId, ct);

        return new CompleteAttemptResponse { AttemptId = attemptId, ReportJson = json };
    }
}
