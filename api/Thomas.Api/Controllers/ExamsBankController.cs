using Microsoft.AspNetCore.Mvc;
using Thomas.Api.Application.Interfaces;

namespace Thomas.Api.Controllers;

[ApiController]
[Route("api/exams")]
public class ExamsBankController : ControllerBase
{
    private readonly IExamBankService _svc;
    public ExamsBankController(IExamBankService svc) => _svc = svc;

    /// <summary>Candidate-safe: practice question bank (no answer keys)</summary>
    [HttpGet("{code}/practice-bank")]
    public async Task<IActionResult> GetPracticeBank(string code, CancellationToken ct)
    {
        var dto = await _svc.GetPracticeBankAsync(code, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Admin-only: full question bank including correct answers</summary>
    [HttpGet("{code}/full")]
    // [Authorize(Roles = "Admin")]   // enable when auth is wired
    public async Task<IActionResult> GetFullBank(string code, CancellationToken ct)
    {
        var dto = await _svc.GetFullBankAdminAsync(code, ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}
