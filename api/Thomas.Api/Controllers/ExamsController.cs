using Microsoft.AspNetCore.Mvc;
using Thomas.Api.Application.Interfaces;

namespace Thomas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly IExamService _service;
    public ExamsController(IExamService service) => _service = service;

    /// <summary>All active exams (basic metadata; no heavy content)</summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken ct)
        => Ok(await _service.GetActiveAsync(ct));

    /// <summary>Single exam by code, includes enabled sections ordered by OrderIndex</summary>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
    {
        var exam = await _service.GetByCodeAsync(code, ct);
        return exam is null ? NotFound() : Ok(exam);
    }
}
