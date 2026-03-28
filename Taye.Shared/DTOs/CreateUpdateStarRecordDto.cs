using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Taye.Shared.DTOs;

/// <summary>
/// 创建/更新星星记录 DTO
/// </summary>
public class CreateUpdateStarRecordDto
{
    [Required(ErrorMessage = "日期不能为空")]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "星星数量不能为空")]
    [Range(-9999, 9999, ErrorMessage = "星星数量范围 -9999 到 9999")]
    public int StarCount { get; set; }

    [Required(ErrorMessage = "原因不能为空")]
    [MaxLength(100, ErrorMessage = "原因不能超过100字")]
    public string Reason { get; set; } = string.Empty;

    [Required(ErrorMessage = "类型不能为空")]
    [RegularExpression("^(Gain|Spend|Punish)$", ErrorMessage = "类型只能是 Gain 或 Spend 或 Punish")]
    public string Type { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "备注不能超过500字")]
    public string? Notes { get; set; }

    // 图片文件（可选）
    public IFormFile? ImageFile { get; set; }
}
