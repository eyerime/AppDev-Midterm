namespace MidtermProject.Models;

public enum WearSeverity
{
    Minor,
    Moderate,
    Major
}

public enum UsageLevel
{
    Light,
    Regular,
    Heavy
}

/// <summary>A cosmetic flaw the seller discloses, e.g. "Back of neck — finish wear".</summary>
public record WearNote(string Area, string Description, WearSeverity Severity);

/// <summary>One line of the functional checklist. A failed check should explain itself in <see cref="Note"/>.</summary>
public record FunctionCheck(string Label, bool Passed, string? Note = null);

/// <summary>
/// The seller's disclosure for a second-hand item: history, flaws, testing, parts and price context.
/// </summary>
public record ConditionReport
{
    public static readonly string[] StandardChecks =
    {
        "Powers on / produces sound",
        "Electronics & controls tested",
        "Hardware & mechanics working",
        "Plays & tunes correctly",
    };

    public int? YearMade { get; init; }

    /// <summary>Total owners including the current seller (1 = original owner).</summary>
    public int OwnerCount { get; init; } = 1;

    public UsageLevel Usage { get; init; } = UsageLevel.Regular;
    public string ReasonForSelling { get; init; } = "";
    public int? OriginalPrice { get; init; }

    public bool NoVisibleWear { get; init; }
    public List<WearNote> Wear { get; init; } = new();
    public List<FunctionCheck> Checks { get; init; } = new();
    public List<string> Included { get; init; } = new();

    /// <summary>Non-original or replaced parts. Empty means everything is original.</summary>
    public List<string> Modifications { get; init; } = new();

    public bool SerialVerified { get; init; }
    public bool HasReceipt { get; init; }

    /// <summary>0–100: how much the seller has disclosed.</summary>
    public int Score
    {
        get
        {
            var score = 0;
            if (YearMade.HasValue) score += 15;
            if (!string.IsNullOrWhiteSpace(ReasonForSelling)) score += 10;
            if (OriginalPrice.HasValue) score += 15;
            if (NoVisibleWear || Wear.Count > 0) score += 20;
            score += Math.Min(3, Checks.Count(c => c.Passed)) * 5;
            if (Included.Count > 0) score += 10;
            if (SerialVerified) score += 10;
            if (HasReceipt) score += 5;
            return score;
        }
    }

    public string ScoreLabel => Score switch
    {
        >= 80 => "Fully disclosed",
        >= 50 => "Well documented",
        _ => "Basic details"
    };

    /// <summary>Percent below original retail (negative when priced above it); null when unknown.</summary>
    public int? PercentOffNew(int price) =>
        OriginalPrice is > 0 ? (int)Math.Round((1 - price / (double)OriginalPrice.Value) * 100) : null;
}

public static class ReportLabels
{
    public static string Label(this UsageLevel usage) => usage switch
    {
        UsageLevel.Light => "Light · mostly home use",
        UsageLevel.Regular => "Regular · practice & rehearsal",
        _ => "Heavy · gigging / touring"
    };

    public static string Label(this WearSeverity severity) => severity switch
    {
        WearSeverity.Minor => "Minor",
        WearSeverity.Moderate => "Moderate",
        _ => "Major"
    };

    public static string OwnerLabel(this int owners) => owners switch
    {
        <= 1 => "Original owner",
        2 => "2 owners",
        3 => "3 owners",
        _ => "4+ owners"
    };
}

/// <summary>The grading scale shown on every listing so "Very Good" means the same thing everywhere.</summary>
public static class ConditionGrades
{
    public static readonly IReadOnlyList<(string Name, string Description)> Scale = new[]
    {
        ("Like New", "Barely used. No visible wear; may include original packaging."),
        ("Excellent", "Light use. Only tiny cosmetic marks, visible up close."),
        ("Very Good", "Regular use. Minor scratches or dings; fully functional."),
        ("Good", "Clearly used. Noticeable cosmetic wear; fully functional."),
        ("Fair", "Heavy wear or small functional quirks — all disclosed in the report."),
    };

    public static readonly string[] SellerOptions = { "Like New", "Excellent", "Very Good", "Good", "Fair", "Vintage", "Restored", "Collector" };

    public static int IndexOf(string condition) =>
        Scale.ToList().FindIndex(g => string.Equals(g.Name, condition, StringComparison.OrdinalIgnoreCase));

    public static string Describe(string condition) => condition switch
    {
        "Vintage" => "Vintage — age-related wear and non-original parts are expected. Read the report below.",
        "Restored" => "Restored — repaired or refinished. Check the modifications list below.",
        "Collector" => "Collector grade — rare or historically significant. Check the report for originality.",
        _ => IndexOf(condition) >= 0 ? Scale[IndexOf(condition)].Description : ""
    };
}
