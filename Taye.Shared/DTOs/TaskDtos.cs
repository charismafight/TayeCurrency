namespace Taye.Shared.DTOs;

public class TaskDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Target { get; set; }
    public int Current { get; set; }
    public bool IsCompleted { get; set; }
}

public class TasksResponseDto
{
    public string Date { get; set; } = string.Empty;
    public List<TaskDto> Tasks { get; set; } = new();
    public bool AllCompleted { get; set; }
    public int BonusStars { get; set; }
    public bool BonusEarned { get; set; }
}
