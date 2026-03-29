namespace Taye.WebAPI.Services;

public interface IReasonTemplateService
{
    Dictionary<string, int> GetRewardTemplates();   // 奖励类
    Dictionary<string, int> GetSpendTemplates();    // 花费类
    Dictionary<string, int> GetPunishTemplates();   // 惩罚类
    Dictionary<string, int> GetTemplatesByType(string type);
}

public class ReasonTemplateService : IReasonTemplateService
{
    // 奖励类（正值）
    private readonly Dictionary<string, int> _rewardTemplates = new()
    {
        { "完成每日任务", 10 },
        { "老师表扬学校优秀表现", 6 },
        { "帮助他人", 15 },
        { "考试获得100分（满分100）", 12 },
        { "考试获得99、98分（满分100）", 6 },
        { "晚上21:30前收好书包，洗漱完毕，换好睡衣，上自己的床", 1 },
    };

    // 花费类（负值）
    private readonly Dictionary<string, int> _spendTemplates = new()
    {
        { "购买零食（每1块钱）", -1 },
        { "购买玩具（每1块钱）", -1 },
        { "玩游戏（每5分钟）", -1 },
    };

    // 惩罚类（负值，较重）
    private readonly Dictionary<string, int> _punishTemplates = new()
    {
        { "晚上22:00前没有收好书包，洗漱完毕，换好睡衣，上自己的床", -1 },
        { "尿尿忘记冲厕所", -1 },
        { "起床或者晚上睡觉前忘记洗脸（或洗澡时忘记洗脸）", -1 },
        { "忘记带上课需要的文具、工具、书等等", -3 },
        { "作业未完成或者各类老师评价得A-以下（不包括A-）", -2 },
        { "说谎", -30 },
        { "打架", -50 },
    };

    public Dictionary<string, int> GetRewardTemplates()
    {
        return _rewardTemplates;
    }

    public Dictionary<string, int> GetSpendTemplates()
    {
        return _spendTemplates;
    }

    public Dictionary<string, int> GetPunishTemplates()
    {
        return _punishTemplates;
    }

    public Dictionary<string, int> GetTemplatesByType(string type)
    {
        return type switch
        {
            "奖励" => _rewardTemplates,
            "花费" => _spendTemplates,
            "惩罚" => _punishTemplates,
            _ => new Dictionary<string, int>()
        };
    }
}
