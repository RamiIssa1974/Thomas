using Microsoft.EntityFrameworkCore;
using Thomas.Api.Domain.Entities;

namespace Thomas.Api.Infrastructure.Repositories;

public class AttemptRepository : IAttemptRepository
{
    private readonly ThomasDbContext _db;
    public AttemptRepository(ThomasDbContext db) => _db = db;

    public Task<Exam?> GetActiveExamByCodeAsync(string code, CancellationToken ct) =>
        _db.Exams.AsNoTracking()
            .Include(e => e.Sections)
            .Include(e => e.PracticeSettings)
            .FirstOrDefaultAsync(e => e.IsActive && e.Code == code, ct);

    public async Task<long> CreateAttemptAsync(Attempt attempt, IEnumerable<AttemptSection> sections, CancellationToken ct)
    {
        // 1) insert attempt first to get identity
        await _db.Attempts.AddAsync(attempt, ct);
        await _db.SaveChangesAsync(ct);

        // 2) set FK on sections and insert
        foreach (var s in sections)
        {
            s.AttemptId = attempt.Id;
            // s.Status should already be NotStarted; keep as-is
        }
        await _db.AttemptSections.AddRangeAsync(sections, ct);
        await _db.SaveChangesAsync(ct);

        return attempt.Id;
    }

    public Task<bool> AttemptExistsAsync(long attemptId, CancellationToken ct) =>
        _db.Attempts.AsNoTracking().AnyAsync(a => a.Id == attemptId, ct);

    public Task<Attempt?> GetAttemptWithExamAsync(long attemptId, CancellationToken ct) =>
        _db.Attempts
           .Include(a => a.Exam).ThenInclude(e => e.Sections)
           .FirstOrDefaultAsync(a => a.Id == attemptId, ct);

    public async Task<List<int>> GetPracticeQuestionIdsPerSectionAsync(int examSectionId, int take, CancellationToken ct)
    {
        return await _db.Questions.AsNoTracking()
            .Where(q => q.ExamSectionId == examSectionId && q.IsPractice && q.IsActive)
            .OrderBy(_ => Guid.NewGuid()) // NEWID()
            .Select(q => q.Id)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<List<int>> GetRealQuestionIdsPerSectionAsync(int examSectionId, int take, CancellationToken ct)
    {
        return await _db.Questions.AsNoTracking()
            .Where(q => q.ExamSectionId == examSectionId && !q.IsPractice && q.IsActive)
            .OrderBy(_ => Guid.NewGuid()) // NEWID()
            .Select(q => q.Id)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task AddAttemptQuestionsAsync(long attemptId, int examSectionId, IReadOnlyList<int> questionIds, CancellationToken ct)
    {
        var items = questionIds.Select((qid, idx) => new AttemptQuestion
        {
            AttemptId = attemptId,
            ExamSectionId = examSectionId,
            QuestionId = qid,
            SequenceIndex = idx + 1
        }).ToList();

        await _db.AttemptQuestions.AddRangeAsync(items, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<(int? questionId, int sequenceIndex)?> GetNextUnansweredAsync(long attemptId, int examSectionId, CancellationToken ct)
    {
        var q = await _db.AttemptQuestions
            .Where(aq => aq.AttemptId == attemptId && aq.ExamSectionId == examSectionId)
            .OrderBy(aq => aq.SequenceIndex)
            .Select(aq => new
            {
                aq.QuestionId,
                aq.SequenceIndex,
                answered = _db.AttemptAnswers.Any(aa => aa.AttemptId == attemptId && aa.QuestionId == aq.QuestionId)
            })
            .FirstOrDefaultAsync(x => !x.answered, ct);

        return q is null ? null : (q.QuestionId, q.SequenceIndex);
    }

    public Task<Question?> GetQuestionWithOptionsAsync(int questionId, CancellationToken ct) =>
        _db.Questions.AsNoTracking()
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == questionId, ct);

    public async Task SaveAnswerAsync(AttemptAnswer answer, CancellationToken ct)
    {
        _db.AttemptAnswers.Add(answer);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<(int raw, int max)> ComputeSectionScoresAsync(long attemptId, int examSectionId, CancellationToken ct)
    {
        var max = await _db.AttemptQuestions
            .Where(aq => aq.AttemptId == attemptId && aq.ExamSectionId == examSectionId)
            .CountAsync(ct);

        var raw = await _db.AttemptAnswers
            .Where(aa => aa.AttemptId == attemptId && aa.ExamSectionId == examSectionId && aa.IsCorrect == true)
            .CountAsync(ct);

        return (raw, max);
    }

    public async Task<List<(int sectionId, string sectionName, int raw, int max)>> ComputeAllSectionsAsync(long attemptId, CancellationToken ct)
    {
        var secs = await _db.ExamSections
            .Join(_db.AttemptSections.Where(x => x.AttemptId == attemptId),
                  s => s.Id, asec => asec.ExamSectionId,
                  (s, asec) => new { s.Id, s.Name })
            .ToListAsync(ct);

        var list = new List<(int, string, int, int)>();
        foreach (var s in secs)
        {
            var (raw, max) = await ComputeSectionScoresAsync(attemptId, s.Id, ct);
            list.Add((s.Id, s.Name, raw, max));
        }
        return list;
    }

    public async Task SaveReportAsync(long attemptId, string json, CancellationToken ct)
    {
        var existing = await _db.Reports.FirstOrDefaultAsync(r => r.AttemptId == attemptId, ct);
        if (existing is null)
            await _db.Reports.AddAsync(new Report { AttemptId = attemptId, SummaryJson = json }, ct);
        else
        {
            existing.SummaryJson = json;
            existing.GeneratedAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task MarkAttemptCompletedAsync(long attemptId, CancellationToken ct)
    {
        var a = await _db.Attempts.FirstAsync(x => x.Id == attemptId, ct);
        a.Status = Domain.Entities.AttemptStatus.Completed;
        a.CompletedAt = DateTime.UtcNow;
        if (a.StartedAt is not null)
            a.TotalTimeSeconds = (int)(a.CompletedAt.Value - a.StartedAt.Value).TotalSeconds;
        await _db.SaveChangesAsync(ct);
    }
}
