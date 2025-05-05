namespace FPT.TeamMatching.Domain.Utilities;

public class Utils
{
    public static string ToCronExpression(DateTimeOffset dateTime, int delayMinutes = 0, int delayHours = 0)
    {
        var adjustedTime = dateTime
            .AddMinutes(delayMinutes)
            .AddHours(delayHours);

        int seconds = adjustedTime.Second;
        int minutes = adjustedTime.Minute;
        int hours = adjustedTime.Hour;
        int day = adjustedTime.Day;
        int month = adjustedTime.Month;

        // Cronos supports 6-part cron: second minute hour day month day-of-week
        // Use '*' for day-of-week
        return $"{seconds} {minutes} {hours} {day} {month} *";
    }

}