using MidtermProject.Data;
using MidtermProject.Models;

namespace MidtermProject.Services;

/// <summary>Fixed-price listings, loaded from Supabase at startup.</summary>
public class CatalogService
{
    private readonly SupabaseDb _db;
    private List<FixedItem> _items = new();

    public CatalogService(SupabaseDb db) => _db = db;

    public async Task LoadAsync() => _items = await _db.LoadItemsAsync();

    public FixedItem? Find(string id) =>
        _items.FirstOrDefault(i => string.Equals(i.Id, id, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<FixedItem> GetYourItems() => _items.Where(i => i.IsYours);

    public void Add(FixedItem item)
    {
        _items.Insert(0, item);
        _ = _db.AddItemAsync(item);
    }

    /// <summary>Listings leave the shop once purchased. In the database this is a soft delete (deleted_at).</summary>
    public void Remove(IEnumerable<string> ids)
    {
        var sold = ids.ToHashSet();
        _items.RemoveAll(i => sold.Contains(i.Id));
        _ = _db.SoftDeleteItemsAsync(sold);
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
