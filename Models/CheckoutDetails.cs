namespace MidtermProject.Models;

/// <summary>Shipping and payment choices entered at checkout (demo only — nothing is charged).</summary>
public record CheckoutDetails(
    string FullName,
    string Email,
    string Address,
    string City,
    string PostalCode,
    string PaymentMethod);
