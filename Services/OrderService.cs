using MidtermProject.Data;
using MidtermProject.Models;

namespace MidtermProject.Services;

/// <summary>Order history (newest first), loaded from Supabase at startup.</summary>
public class OrderService
{
    private readonly SupabaseDb _db;
    private List<Order> _orders = new();

    public OrderService(SupabaseDb db) => _db = db;

    public async Task LoadAsync() => _orders = await _db.LoadOrdersAsync();

    public IReadOnlyList<Order> Orders => _orders;

    public Order? Get(string number) =>
        _orders.FirstOrDefault(o => string.Equals(o.Number, number, StringComparison.OrdinalIgnoreCase));

    public Order Place(IReadOnlyList<FixedItem> items, int shipping, CheckoutDetails details)
    {
        var name = items.Count == 1 ? items[0].Name : $"{items[0].Name} + {items.Count - 1} more";
        var order = new Order(name, "Processing · placed just now", items.Sum(i => i.Price) + shipping)
        {
            Number = "RS-" + Random.Shared.Next(10000, 99999),
            Items = items.ToList(),
            Shipping = shipping,
            Details = details,
        };
        _orders.Insert(0, order);
        _ = _db.AddOrderAsync(order);
        return order;
    }
}
