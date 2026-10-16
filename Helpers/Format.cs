using System.Globalization;

namespace MidtermProject.Helpers;

/// <summary>Display formatting shared by components.</summary>
public static class Format
{
    public static string Money(int amount) =>
        "$" + amount.ToString("N0", CultureInfo.InvariantCulture);

    /// <summary>Formats a duration as HH:MM:SS (hours may exceed 24).</summary>
    public static string Countdown(TimeSpan span)
    {
        var total = (int)Math.Max(0, span.TotalSeconds);
        var h = total / 3600;
        var m = total % 3600 / 60;
        var s = total % 60;
        return $"{h:00}:{m:00}:{s:00}";
    }
}
