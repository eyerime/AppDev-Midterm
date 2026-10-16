namespace MidtermProject.Models;

/// <summary>One line in an auction's bid history.</summary>
public record BidEntry(string Who, int Amount, string Ago)
{
    public const string YouLabel = "You";

    public bool IsYou => Who == YouLabel;
}
