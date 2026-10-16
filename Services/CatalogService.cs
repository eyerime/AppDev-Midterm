using MidtermProject.Data;
using MidtermProject.Models;

namespace MidtermProject.Services;

/// <summary>In-memory fixed-price listings.</summary>
public class CatalogService
{
    private readonly List<FixedItem> _items = SeedData.CreateFixedItems().ToList();

    public FixedItem? Find(string id) =>
        _items.FirstOrDefault(i => string.Equals(i.Id, id, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<FixedItem> GetYourItems() => _items.Where(i => i.IsYours);

    public void Add(FixedItem item) => _items.Insert(0, item);

    /// <summary>Removes listings once they have been purchased.</summary>
    public void Remove(IEnumerable<string> ids)
    {
        var sold = ids.ToHashSet();
        _items.RemoveAll(i => sold.Contains(i.Id));
    }

    /// <summary>A specific category wins over a group; with neither, everything is returned.</summary>
    public IEnumerable<FixedItem> GetItems(ItemCategory? category = null, CategoryGroup? group = null, string? query = null)
    {
        IEnumerable<FixedItem> result = _items;
        if (category is { } c) result = result.Where(i => i.Category == c);
        else if (group is { } g) result = result.Where(i => i.Category.Group() == g);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            result = result.Where(i => terms.All(t =>
                i.Name.Contains(t, StringComparison.OrdinalIgnoreCase)
                || i.Brand.Contains(t, StringComparison.OrdinalIgnoreCase)
                || i.Category.DisplayName().Contains(t, StringComparison.OrdinalIgnoreCase)));
        }

        return result;
    }
}
