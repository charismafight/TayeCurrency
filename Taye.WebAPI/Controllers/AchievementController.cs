using Microsoft.AspNetCore.Mvc;
using Taye.Shared.DTOs;
using Taye.WebAPI.Services;

namespace Taye.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AchievementController : ControllerBase
{
    private readonly IAchievementService _achievementService;
    private readonly ILogger<AchievementController> _logger;

    public AchievementController(IAchievementService achievementService, ILogger<AchievementController> logger)
    {
        _achievementService = achievementService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<List<AchievementDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<List<AchievementDto>>>> GetAchievements([FromQuery] string? userId = null)
    {
        try
        {
            var result = await _achievementService.GetPlayerAchievementsAsync(userId);
            return Ok(APIResponse<List<AchievementDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取成就列表失败");
            return StatusCode(500, APIResponse<List<AchievementDto>>.Fail("获取成就失败：" + ex.Message));
        }
    }
}
