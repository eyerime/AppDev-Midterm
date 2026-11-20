namespace MidtermProject.Models;

/// <summary>A purchase shown on the profile page and the confirmation screen.</summary>
public record Order(string ItemName, string StatusText, int Total)
{
    /// <summary>Empty for the pre-seeded history orders.</summary>
    public string Number { get; init; } = "";

    public IReadOnlyList<FixedItem> Items { get; init; } = Array.Empty<FixedItem>();
    public int Shipping { get; init; }
    public CheckoutDetails? Details { get; init; }
}
