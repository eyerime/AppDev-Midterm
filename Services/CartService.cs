using MidtermProject.Models;

namespace MidtermProject.Services;

/// <summary>Shopping cart. Used instruments are one-of-a-kind, so each item can be added once.</summary>
public class CartService
{
    public const int ShippingFee = 25;

    private readonly List<FixedItem> _items = new();

    public event Action? Changed;

    public IReadOnlyList<FixedItem> Items => _items;
    public int Count => _items.Count;
    public int Subtotal => _items.Sum(i => i.Price);
    public int Shipping => _items.Count == 0 ? 0 : ShippingFee;
    public int Total => Subtotal + Shipping;

    public bool Contains(string id) => _items.Any(i => i.Id == id);

    public bool Add(FixedItem item)
    {
        if (Contains(item.Id)) return false;
        _items.Add(item);
        Changed?.Invoke();
        return true;
    }

    public void Remove(string id)
    {
        if (_items.RemoveAll(i => i.Id == id) > 0)
            Changed?.Invoke();
    }

    public void Clear()
    {
        _items.Clear();
        Changed?.Invoke();
    }
}
