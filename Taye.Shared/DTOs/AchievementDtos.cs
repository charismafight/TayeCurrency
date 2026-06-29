namespace Taye.Shared.DTOs;

public class MilestoneDto
{
    public int Count { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Bonus { get; set; }
}

public class AchievementDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Category { get; set; }
    public int CurrentCount { get; set; }
    public List<MilestoneDto> Milestones { get; set; } = new();
    public int UnlockedMilestoneIndex { get; set; } = -1;
    public bool IsUnlocked { get; set; }
    public bool IsHidden { get; set; }
    public string? NextMilestoneTitle { get; set; }
    public int? NextMilestoneCount { get; set; }
}
