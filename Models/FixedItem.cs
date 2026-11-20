namespace MidtermProject.Models;

/// <summary>A fixed-price marketplace listing.</summary>
public record FixedItem(
    string Id,
    string Name,
    string Brand,
    ItemCategory Category,
    string Condition,
    int Price,
    string Img)
{
    /// <summary>True when listed by the signed-in member.</summary>
    public bool IsYours { get; init; }

    /// <summary>Second-hand disclosure; null for listings without one.</summary>
    public ConditionReport? Report { get; init; }
}
