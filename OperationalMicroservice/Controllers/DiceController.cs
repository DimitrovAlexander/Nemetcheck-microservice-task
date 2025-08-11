using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OperationalMicroservice.Data.DTOs;
using OperationalMicroservice.Services.DiceService;

namespace OperationalMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DiceController : ControllerBase
    {
        private readonly IDiceService _diceService;
        private readonly ILogger<DiceController> _logger;

        public DiceController(IDiceService diceService, ILogger<DiceController> logger)
        {
            _diceService = diceService;
            _logger = logger;
        }

        // POST /api/dice/roll
        [HttpPost("roll")]
        public async Task<IActionResult> Roll()
        {
            var userId = GetUserIdFromClaims();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _diceService.RollAsync(userId);
            return Ok(result);
        }

        // GET /api/dice/history
        [HttpGet("history")]
        public async Task<IActionResult> History([FromQuery] HistoryQueryParams query)
        {
            var userId = GetUserIdFromClaims();
            if (userId == Guid.Empty) return Unauthorized();

            var paged = await _diceService.GetHistoryAsync(userId, query);
            return Ok(paged);
        }

        private Guid GetUserIdFromClaims()
        {
            // Look for "sub" claim (JwtRegisteredClaimNames.Sub) or NameIdentifier
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (Guid.TryParse(sub, out var id)) return id;
            return Guid.Empty;
        }
    }
}
