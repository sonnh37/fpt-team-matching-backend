namespace FPT.TeamMatching.Domain.Utilities;

public class Utils
{
    public static string ToCronExpression(DateTimeOffset dateTime, bool includeSeconds = false, int delayMinute = 0)
    {
        // Standard cron format: 
        // [second] [minute] [hour] [day of month] [month] [day of week]
        // If includeSeconds is false, the seconds field is omitted
        
        string seconds = dateTime.Second.ToString();
        string minutes = dateTime.Minute.ToString();
        string hours = dateTime.Hour.ToString();
        string dayOfMonth = dateTime.Day.ToString();
        string month = (dateTime.Month).ToString();
        string dayOfWeek = ((int)dateTime.DayOfWeek).ToString();
        
        if (includeSeconds)
        {
            // Format with seconds: [second] [minute] [hour] [day of month] [month] [day of week]
            return $"{seconds} {minutes+delayMinute} {hours} {dayOfMonth} {month} {dayOfWeek}";
        }
        else
        {
            // Format without seconds: [minute] [hour] [day of month] [month] [day of week]
            return $"{delayMinute} {0} {dayOfMonth} {month} {dayOfWeek}";
        }
    }
}