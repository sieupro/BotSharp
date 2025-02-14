using BotSharp.Abstraction.Statistics.Enums;

namespace BotSharp.Abstraction.Statistics.Models;

public class BotSharpStats
{
    [JsonPropertyName("metric")]
    public string Metric { get; set; } = null!;

    [JsonPropertyName("dimension")]
    public string Dimension { get; set; } = null!;

    [JsonPropertyName("value")]
    public string Value { get; set; } = null!;

    [JsonPropertyName("data")]
    public IDictionary<string, double> Data { get; set; } = new Dictionary<string, double>();

    [JsonPropertyName("record_time")]
    public DateTime RecordTime { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public StatsInterval IntervalType { get; set; }

    [JsonPropertyName("interval")]
    public string Interval
    {
        get
        {
            return IntervalType.ToString();
        } 
        set
        {
            if (Enum.TryParse(value, out StatsInterval type))
            {
                IntervalType = type;
            }
        }
    }

    [JsonPropertyName("start_time")]
    public DateTime StartTime { get; set; }

    [JsonPropertyName("end_time")]
    public DateTime EndTime { get; set; }

    public override string ToString()
    {
        return $"{Metric}-{Dimension}-{Value} ({Interval}): {Data?.Count ?? 0}";
    }

    public static (DateTime, DateTime) BuildTimeInterval(DateTime recordTime, StatsInterval interval)
    {
        DateTime startTime = recordTime;
        DateTime endTime = DateTime.UtcNow;

        switch (interval)
        {
            case StatsInterval.Hour:
                startTime = new DateTime(recordTime.Year, recordTime.Month, recordTime.Day, recordTime.Hour, 0, 0);
                endTime = startTime.AddHours(1);
                break;
            case StatsInterval.Week:
                var dayOfWeek = startTime.DayOfWeek;
                var firstDayOfWeek = startTime.AddDays(-(int)dayOfWeek);
                startTime = new DateTime(firstDayOfWeek.Year, firstDayOfWeek.Month, firstDayOfWeek.Day, 0, 0, 0);
                endTime = startTime.AddDays(7);
                break;
            case StatsInterval.Month:
                startTime = new DateTime(recordTime.Year, recordTime.Month, 1);
                endTime = startTime.AddMonths(1);
                break;
            default:
                startTime = new DateTime(recordTime.Year, recordTime.Month, recordTime.Day, 0, 0, 0);
                endTime = startTime.AddDays(1);
                break;
        }

        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        endTime = DateTime.SpecifyKind(endTime, DateTimeKind.Utc);
        return (startTime, endTime);
    }
}