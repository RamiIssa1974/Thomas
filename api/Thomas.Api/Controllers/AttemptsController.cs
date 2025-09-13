using Microsoft.AspNetCore.Mvc;
using Thomas.Api.Application.Dtos;
using Thomas.Api.Application.Interfaces;

namespace Thomas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttemptsController : ControllerBase
{
    private readonly IAttemptService _svc;
    public AttemptsController(IAttemptService svc) => _svc = svc;

    [HttpPost("{attemptId:long}/answer")]
    public async Task<IActionResult> Answer(long attemptId, [FromQuery] AttemptModeDto mode, [FromBody] SubmitAnswerRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _svc.SubmitAnswerAsync(attemptId, mode, req, ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }


    [HttpPost("{attemptId:long}/sections/{sectionId:int}/start")]
    public async Task<IActionResult> StartSection(long attemptId, int sectionId, CancellationToken ct)
    {
        await _svc.StartSectionAsync(attemptId, sectionId, ct);
        return NoContent();
    }


    // In real app you'd pull the userId from auth; for now, a fixed dev user:

    private static readonly Guid DevUser = Guid.Parse("808FBC1B-9F90-F011-8083-0050560BE96F");


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttemptRequest req, CancellationToken ct)
    {
        var result = await _svc.CreateAsync(req, DevUser, ct);
        return Ok(result);
    }

    [HttpGet("{attemptId:long}/sections/{sectionId:int}/next")]
    public async Task<IActionResult> Next(long attemptId, int sectionId, CancellationToken ct)
    {
        var q = await _svc.GetNextQuestionAsync(attemptId, sectionId, ct);
        return q is null ? NoContent() : Ok(q);
    }
     
    [HttpPost("{attemptId:long}/complete")]
    public async Task<IActionResult> Complete(long attemptId, CancellationToken ct)
    {
        var report = await _svc.CompleteAsync(attemptId, ct);
        return Ok(report);
    }
}
