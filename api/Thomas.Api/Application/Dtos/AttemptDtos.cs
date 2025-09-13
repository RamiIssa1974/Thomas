namespace Thomas.Api.Application.Dtos;

public enum AttemptModeDto { Practice = 0, Real = 1 }

public sealed class CreateAttemptRequest
{
    public string ExamCode { get; set; } = "";
    public AttemptModeDto Mode { get; set; } = AttemptModeDto.Practice;

    // consents (pre-exam screen)
    public bool ConsentDataPrivacy { get; set; }
    public bool ConsentNoCheating { get; set; }
    public bool ConsentTimeLimits { get; set; }
    public bool ConsentResultsUsage { get; set; }

    // optional client info
    public string? ClientUserAgent { get; set; }
    public string? ClientIp { get; set; }
}

public sealed class CreateAttemptResponse
{
    public long AttemptId { get; set; }
    public string ExamCode { get; set; } = "";
    public AttemptModeDto Mode { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public IReadOnlyList<AttemptSectionSummaryDto> Sections { get; set; } = Array.Empty<AttemptSectionSummaryDto>();
}

public sealed class AttemptSectionSummaryDto
{
    public int ExamSectionId { get; set; }
    public string SectionName { get; set; } = "";
    public int PlannedQuestions { get; set; }
}

public sealed class NextQuestionDto
{
    public int ExamSectionId { get; set; }
    public int QuestionId { get; set; }
    public string Stem { get; set; } = "";
    public int OrderInSection { get; set; } // sequence index in this attempt
    public string Type { get; set; } = "SingleChoice";
    public List<QuestionOptionLiteDto> Options { get; set; } = new();
}

public sealed class QuestionOptionLiteDto
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public int OrderIndex { get; set; }
}

public sealed class SubmitAnswerRequest
{
    public int ExamSectionId { get; set; }
    public int QuestionId { get; set; }
    public List<int>? SelectedOptionIds { get; set; } // for choice questions
    public decimal? NumericValue { get; set; }        // future-proof
    public int? TimeToAnswerMs { get; set; }
}

public sealed class SubmitAnswerResult
{
    public bool IsCorrect { get; set; }
    public List<int>? CorrectOptionIds { get; set; } // null for Real
}

public sealed class CompleteAttemptResponse
{
    public long AttemptId { get; set; }
    public string ReportJson { get; set; } = "{}";
}
