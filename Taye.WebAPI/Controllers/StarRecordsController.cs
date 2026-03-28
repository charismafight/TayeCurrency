using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taye.Shared.Entities;
using Taye.Shared.DTOs;
using Taye.WebAPI.Data;
using Taye.WebAPI.Services;

namespace Taye.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StarRecordsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<StarRecordsController> _logger;
    private readonly IFileUploadService _fileUploadService;

    public StarRecordsController(AppDbContext context, ILogger<StarRecordsController> logger, IFileUploadService fileUploadService)
    {
        _context = context;
        _logger = logger;
        _fileUploadService = fileUploadService;
    }

    /// <summary>
    /// 获取所有星星记录
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(APIResponse<List<StarRecordDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<List<StarRecordDto>>>> GetRecords(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? type,
        [FromQuery] string? userId = null)
    {
        try
        {
            var query = _context.StarRecords.AsQueryable();

            // 日期范围过滤
            if (startDate.HasValue)
                query = query.Where(r => r.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(r => r.Date <= endDate.Value);

            // 类型过滤
            if (!string.IsNullOrEmpty(type))
                query = query.Where(r => r.Type == type);

            // 用户过滤（多用户支持）
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(r => r.UserId == userId);

            var records = await query
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.CreatedAt)
                .Select(r => new StarRecordDto
                {
                    Id = r.Id,
                    Date = r.Date,
                    StarCount = r.StarCount,
                    Reason = r.Reason,
                    Type = r.Type,
                    Notes = r.Notes
                })
                .ToListAsync();

            return Ok(APIResponse<List<StarRecordDto>>.Ok(records));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取星星记录失败");
            return StatusCode(500, APIResponse<List<StarRecordDto>>.Fail("获取记录失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 分页获取星星记录（支持分页）
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(APIResponse<PagedResult<StarRecordDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<PagedResult<StarRecordDto>>>> GetRecordsPaged(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? type,
        [FromQuery] string? userId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            // 限制最大每页数量
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            var query = _context.StarRecords.AsQueryable();

            // 日期范围过滤
            if (startDate.HasValue)
                query = query.Where(r => r.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(r => r.Date <= endDate.Value);

            // 类型过滤
            if (!string.IsNullOrEmpty(type))
                query = query.Where(r => r.Type == type);

            // 用户过滤（多用户支持）
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(r => r.UserId == userId);

            // 获取总记录数
            var totalCount = await query.CountAsync();

            // 分页查询
            var records = await query
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new StarRecordDto
                {
                    Id = r.Id,
                    Date = r.Date,
                    StarCount = r.StarCount,
                    Reason = r.Reason,
                    Type = r.Type,
                    Notes = r.Notes
                })
                .ToListAsync();

            var result = new PagedResult<StarRecordDto>
            {
                Items = records,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(APIResponse<PagedResult<StarRecordDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分页获取星星记录失败");
            return StatusCode(500, APIResponse<PagedResult<StarRecordDto>>.Fail("获取记录失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 根据ID获取单条记录
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(APIResponse<StarRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse<StarRecordDto>>> GetRecord(int id)
    {
        try
        {
            var record = await _context.StarRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound(APIResponse<StarRecordDto>.Fail("记录不存在"));
            }

            var dto = new StarRecordDto
            {
                Id = record.Id,
                Date = record.Date,
                StarCount = record.StarCount,
                Reason = record.Reason,
                Type = record.Type,
                Notes = record.Notes
            };

            return Ok(APIResponse<StarRecordDto>.Ok(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取单条记录失败，ID: {Id}", id);
            return StatusCode(500, APIResponse<StarRecordDto>.Fail("获取记录失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 创建新的星星记录（支持图片上传）
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(APIResponse<StarRecordDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse<StarRecordDto>>> CreateRecord(
        [FromForm] CreateUpdateStarRecordDto createDto)
    {
        try
        {
            // 验证数据
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(APIResponse<StarRecordDto>.Fail(errors));
            }

            // 保存图片（如果有）
            string? imagePath = null;
            string? imageFileName = null;

            if (createDto.ImageFile != null)
            {
                try
                {
                    imagePath = await _fileUploadService.SaveImageAsync(createDto.ImageFile, "star-images");
                    imageFileName = createDto.ImageFile.FileName;
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(APIResponse<StarRecordDto>.Fail(ex.Message));
                }
            }

            // 创建实体
            var record = new StarRecord
            {
                Date = createDto.Date,
                StarCount = createDto.StarCount,
                Reason = createDto.Reason,
                Type = createDto.Type,
                Notes = createDto.Notes,
                ImagePath = imagePath,
                ImageFileName = imageFileName,
                CreatedAt = DateTime.UtcNow,
            };

            _context.StarRecords.Add(record);
            await _context.SaveChangesAsync();

            // 返回 DTO
            var dto = new StarRecordDto
            {
                Id = record.Id,
                Date = record.Date,
                StarCount = record.StarCount,
                Reason = record.Reason,
                Type = record.Type,
                Notes = record.Notes,
                ImagePath = record.ImagePath,
                ImageFileName = record.ImageFileName
            };

            return CreatedAtAction(nameof(GetRecord), new { id = record.Id },
                APIResponse<StarRecordDto>.Ok(dto, "创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建记录失败");
            return StatusCode(500, APIResponse<StarRecordDto>.Fail("创建失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 更新星星记录（支持图片上传）
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(APIResponse<StarRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<APIResponse<StarRecordDto>>> UpdateRecord(
        int id,
        [FromForm] CreateUpdateStarRecordDto updateDto)
    {
        try
        {
            // 查找记录
            var record = await _context.StarRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound(APIResponse<StarRecordDto>.Fail("记录不存在"));
            }

            // 验证数据
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(APIResponse<StarRecordDto>.Fail(errors));
            }

            // 处理图片更新
            if (updateDto.ImageFile != null)
            {
                // 删除旧图片
                if (!string.IsNullOrEmpty(record.ImagePath))
                {
                    _fileUploadService.DeleteImage(record.ImagePath);
                }

                // 保存新图片
                try
                {
                    record.ImagePath = await _fileUploadService.SaveImageAsync(updateDto.ImageFile, "star-images");
                    record.ImageFileName = updateDto.ImageFile.FileName;
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(APIResponse<StarRecordDto>.Fail(ex.Message));
                }
            }

            // 更新实体
            record.Date = updateDto.Date;
            record.StarCount = updateDto.StarCount;
            record.Reason = updateDto.Reason;
            record.Type = updateDto.Type;
            record.Notes = updateDto.Notes;
            record.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 返回 DTO
            var dto = new StarRecordDto
            {
                Id = record.Id,
                Date = record.Date,
                StarCount = record.StarCount,
                Reason = record.Reason,
                Type = record.Type,
                Notes = record.Notes,
                ImagePath = record.ImagePath,
                ImageFileName = record.ImageFileName
            };

            return Ok(APIResponse<StarRecordDto>.Ok(dto, "更新成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新记录失败，ID: {Id}", id);
            return StatusCode(500, APIResponse<StarRecordDto>.Fail("更新失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 删除星星记录（软删除，同时删除图片）
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse<bool>>> DeleteRecord(int id)
    {
        try
        {
            var record = await _context.StarRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound(APIResponse<bool>.Fail("记录不存在"));
            }

            // 软删除
            record.IsDeleted = true;
            record.UpdatedAt = DateTime.UtcNow;

            // 注意：软删除时不删除图片，保留备份

            await _context.SaveChangesAsync();

            return Ok(APIResponse<bool>.Ok(true, "删除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除记录失败，ID: {Id}", id);
            return StatusCode(500, APIResponse<bool>.Fail("删除失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 物理删除（永久删除，同时删除图片文件）
    /// </summary>
    [HttpDelete("{id}/hard")]
    [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<APIResponse<bool>>> HardDeleteRecord(int id)
    {
        try
        {
            var record = await _context.StarRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound(APIResponse<bool>.Fail("记录不存在"));
            }

            // 删除关联的图片文件
            if (!string.IsNullOrEmpty(record.ImagePath))
            {
                _fileUploadService.DeleteImage(record.ImagePath);
            }

            _context.StarRecords.Remove(record);
            await _context.SaveChangesAsync();

            return Ok(APIResponse<bool>.Ok(true, "永久删除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "物理删除失败，ID: {Id}", id);
            return StatusCode(500, APIResponse<bool>.Fail("删除失败：" + ex.Message));
        }
    }

    /// <summary>
    /// 获取统计数据
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(APIResponse<StarStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse<StarStatisticsDto>>> GetStatistics(
        [FromQuery] string? userId = null)
    {
        try
        {
            var query = _context.StarRecords.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(r => r.UserId == userId);

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // 统计数据
            var totalGain = await query.Where(r => r.Type == "Gain").SumAsync(r => r.StarCount);
            var totalSpend = await query.Where(r => r.Type == "Spend").SumAsync(r => r.StarCount);

            var todayGain = await query.Where(r => r.Date == today && r.Type == "Gain")
                .SumAsync(r => r.StarCount);
            var todaySpend = await query.Where(r => r.Date == today && r.Type == "Spend")
                .SumAsync(r => r.StarCount);

            var weekGain = await query.Where(r => r.Date >= startOfWeek && r.Type == "Gain")
                .SumAsync(r => r.StarCount);
            var weekSpend = await query.Where(r => r.Date >= startOfWeek && r.Type == "Spend")
                .SumAsync(r => r.StarCount);

            var monthGain = await query.Where(r => r.Date >= startOfMonth && r.Type == "Gain")
                .SumAsync(r => r.StarCount);
            var monthSpend = await query.Where(r => r.Date >= startOfMonth && r.Type == "Spend")
                .SumAsync(r => r.StarCount);

            // 最近7天数据
            var last7Days = new List<DailyStarDto>();
            for (int i = 6; i >= 0; i--)
            {
                var day = today.AddDays(-i);
                var dayGain = await query.Where(r => r.Date == day && r.Type == "Gain")
                    .SumAsync(r => r.StarCount);
                var daySpend = await query.Where(r => r.Date == day && r.Type == "Spend")
                    .SumAsync(r => r.StarCount);

                last7Days.Add(new DailyStarDto
                {
                    Date = day,
                    Gain = dayGain,
                    Spend = daySpend
                });
            }

            var statistics = new StarStatisticsDto
            {
                TotalGain = totalGain,
                TotalSpend = totalSpend,
                TodayGain = todayGain,
                TodaySpend = todaySpend,
                WeekGain = weekGain,
                WeekSpend = weekSpend,
                MonthGain = monthGain,
                MonthSpend = monthSpend,
                RecentDays = last7Days
            };

            return Ok(APIResponse<StarStatisticsDto>.Ok(statistics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取统计数据失败");
            return StatusCode(500, APIResponse<StarStatisticsDto>.Fail("获取统计失败：" + ex.Message));
        }
    }
}
