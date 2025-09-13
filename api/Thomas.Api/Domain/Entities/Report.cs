namespace Thomas.Api.Domain.Entities;

public class Report
{
    public long Id { get; set; }

    public long AttemptId { get; set; }
    public Attempt Attempt { get; set; } = null!;

    public string SummaryJson { get; set; } = "{}";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
