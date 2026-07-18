using EnterpriseERP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HealthController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("/health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "ok",
                app = "EnterpriseERP",
                environment = _environment.EnvironmentName,
                timeUtc = DateTime.UtcNow
            });
        }

        [HttpGet("/health/ready")]
        public async Task<IActionResult> Ready(CancellationToken cancellationToken)
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

            return canConnect
                ? Ok(new { status = "ready", database = "ok", timeUtc = DateTime.UtcNow })
                : StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "not_ready", database = "unavailable" });
        }
    }
}
