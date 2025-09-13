using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thomas.Api.Infrastructure;

namespace Thomas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ThomasDbContext _db;
    public HealthController(ThomasDbContext db) => _db = db;

    [HttpGet] public IActionResult Ping() => Ok(new { ok = true, utc = DateTime.UtcNow });

    [HttpGet("db")]
    public async Task<IActionResult> Db()
    {
        var can = await _db.Database.CanConnectAsync();
        return can ? Ok(new { canConnect = true })
                   : StatusCode(500, new { canConnect = false });
    }
}
