using MidtermProject.Data;
using MidtermProject.Models;

namespace MidtermProject.Services;

/// <summary>In-memory order history. Newest orders first.</summary>
public class OrderService
{
    private readonly List<Order> _orders = SeedData.Orders.ToList();

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
        return order;
    }
}
