using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Taye.Shared.DTOs;
using Taye.WebAPI.Services;

namespace Taye.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LevelConfigsController : ControllerBase
{
    private readonly ILevelConfigService _levelConfigService;
    private readonly ILogger<LevelConfigsController> _logger;

    public LevelConfigsController(
        ILevelConfigService levelConfigService,
        ILogger<LevelConfigsController> logger)
    {
        _levelConfigService = levelConfigService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<List<LevelConfigDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<List<LevelConfigDto>>>> GetAll()
    {
        try
        {
            var levels = await _levelConfigService.GetAllLevelsAsync();
            return Ok(APIResponse<List<LevelConfigDto>>.Ok(levels));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取等级配置失败");
            return StatusCode(500, APIResponse<List<LevelConfigDto>>.Fail("获取失败：" + ex.Message));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(APIResponse<LevelConfigDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse<LevelConfigDto>>> GetById(int id)
    {
        try
        {
            var level = await _levelConfigService.GetLevelByIdAsync(id);
            if (level == null)
            {
                return NotFound(APIResponse<LevelConfigDto>.Fail("等级配置不存在"));
            }
            return Ok(APIResponse<LevelConfigDto>.Ok(level));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取等级配置失败，ID: {Id}", id);
            return StatusCode(500, APIResponse<LevelConfigDto>.Fail("获取失败：" + ex.Message));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<LevelConfigDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse<LevelConfigDto>>> Create(LevelConfigDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(APIResponse<LevelConfigDto>.Fail("数据验证失败"));
            }

            var result = await _levelConfigService.CreateLevelAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id },
                APIResponse<LevelConfigDto>.Ok(result, "创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建等级配置失败");
            return StatusCode(500, APIResponse<LevelConfigDto>.Fail("创建失败：" + ex.Message));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(APIResponse<LevelConfigDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse<LevelConfigDto>>> Update(int id, LevelConfigDto dto)
    {
        try
        {
            var result = await _levelConfigService.UpdateLevelAsync(id, dto);
            if (result == null)
            {
                return NotFound(APIResponse<LevelConfigDto>.Fail("等级配置不存在"));
            }
            return Ok(APIResponse<LevelConfigDto>.Ok(result, "更新成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新等级配置失败，ID: {Id}", id);
            return StatusCode(500, APIResponse<LevelConfigDto>.Fail("更新失败：" + ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse<bool>>> Delete(int id)
    {
        try
        {
            var result = await _levelConfigService.DeleteLevelAsync(id);
            if (!result)
            {
                return NotFound(APIResponse<bool>.Fail("等级配置不存在"));
            }
            return Ok(APIResponse<bool>.Ok(true, "删除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除等级配置失败，ID: {Id}", id);
            return StatusCode(500, APIResponse<bool>.Fail("删除失败：" + ex.Message));
        }
    }
}
