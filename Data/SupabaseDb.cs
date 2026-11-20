using MidtermProject.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using static Supabase.Postgrest.Constants;

namespace MidtermProject.Data;

// ---------------------------------------------------------------------------
// Row classes: one per table. Your domain models (init-only props, positional
// records) can't be used by the Supabase client directly, so we map to/from them.
// A misspelled [Column] name leaves the property empty with no error.
// ---------------------------------------------------------------------------

[Table("fixed_items")]
public class FixedItemRow : BaseModel
{
    [PrimaryKey("id", true)] public string Id { get; set; } = "";
    [Column("name")] public string Name { get; set; } = "";
    [Column("brand")] public string Brand { get; set; } = "";
    [Column("category")] public string Category { get; set; } = "";
    [Column("condition")] public string Condition { get; set; } = "";
    [Column("price")] public int Price { get; set; }
    [Column("img")] public string Img { get; set; } = "";
    [Column("is_yours")] public bool IsYours { get; set; }
    [Column("report")] public ConditionReport? Report { get; set; }
    [Column("deleted_at")] public DateTime? DeletedAt { get; set; }
    [Column("created_at", ignoreOnInsert: true)] public DateTimeOffset CreatedAt { get; set; }

    public static FixedItemRow From(FixedItem i) => new()
    {
        Id = i.Id, Name = i.Name, Brand = i.Brand, Category = i.Category.ToString(),
        Condition = i.Condition, Price = i.Price, Img = i.Img, IsYours = i.IsYours, Report = i.Report
    };

    public FixedItem ToModel() =>
        new(Id, Name, Brand, Enum.Parse<ItemCategory>(Category), Condition, Price, Img)
        { IsYours = IsYours, Report = Report };
}

[Table("auctions")]
public class AuctionRow : BaseModel
{
    [PrimaryKey("id", true)] public string Id { get; set; } = "";
    [Column("name")] public string Name { get; set; } = "";
    [Column("brand")] public string Brand { get; set; } = "";
    [Column("condition")] public string Condition { get; set; } = "";
    [Column("img")] public string Img { get; set; } = "";
    [Column("starting_bid")] public int StartingBid { get; set; }
    [Column("ends_at_utc")] public DateTimeOffset EndsAtUtc { get; set; }
    [Column("is_yours")] public bool IsYours { get; set; }
    [Column("bid_count")] public int BidCount { get; set; }
    [Column("leader")] public string Leader { get; set; } = nameof(BidLeader.Other);
    [Column("your_bid")] public int? YourBid { get; set; }
    [Column("report")] public ConditionReport? Report { get; set; }

    public static AuctionRow From(Auction a) => new()
    {
        Id = a.Id, Name = a.Name, Brand = a.Brand, Condition = a.Condition, Img = a.Img,
        StartingBid = a.StartingBid,
        EndsAtUtc = new DateTimeOffset(DateTime.SpecifyKind(a.EndsAtUtc, DateTimeKind.Utc)),
        IsYours = a.IsYours, BidCount = a.BidCount, Leader = a.Leader.ToString(),
        YourBid = a.YourBid, Report = a.Report
    };

    public Auction ToModel(List<BidEntry> history) => new()
    {
        Id = Id, Name = Name, Brand = Brand, Condition = Condition, Img = Img,
        StartingBid = StartingBid, EndsAtUtc = EndsAtUtc.UtcDateTime, IsYours = IsYours,
        Report = Report, BidCount = BidCount, Leader = Enum.Parse<BidLeader>(Leader),
        YourBid = YourBid, History = history
    };
}

[Table("bids")]
public class BidRow : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("auction_id")] public string AuctionId { get; set; } = "";
    [Column("who")] public string Who { get; set; } = "";
    [Column("amount")] public int Amount { get; set; }
    [Column("ago_label")] public string? AgoLabel { get; set; }          // seeded rows only
    [Column("created_at", ignoreOnInsert: true)] public DateTimeOffset CreatedAt { get; set; }

    public BidEntry ToModel() => new(Who, Amount, AgoLabel ?? TimeAgo(CreatedAt));

    private static string TimeAgo(DateTimeOffset t)
    {
        var d = DateTimeOffset.UtcNow - t;
        if (d.TotalMinutes < 1) return "just now";
        if (d.TotalHours < 1) return $"{(int)d.TotalMinutes}m ago";
        if (d.TotalDays < 1) return $"{(int)d.TotalHours}h ago";
        return $"{(int)d.TotalDays}d ago";
    }
}

[Table("orders")]
public class OrderRow : BaseModel
{
    // Seeded history orders have an empty Number in the app, but the DB needs a key.
    private const string SeedPrefix = "seed-";

    [PrimaryKey("number", true)] public string Number { get; set; } = "";
    [Column("item_name")] public string ItemName { get; set; } = "";
    [Column("status_text")] public string StatusText { get; set; } = "";
    [Column("total")] public int Total { get; set; }
    [Column("shipping")] public int Shipping { get; set; }
    [Column("details")] public CheckoutDetails? Details { get; set; }
    [Column("created_at")] public DateTimeOffset CreatedAt { get; set; }

    public static OrderRow From(Order o, DateTimeOffset createdAt, int seedIndex = -1) => new()
    {
        Number = string.IsNullOrEmpty(o.Number) ? SeedPrefix + seedIndex : o.Number,
        ItemName = o.ItemName, StatusText = o.StatusText, Total = o.Total,
        Shipping = o.Shipping, Details = o.Details, CreatedAt = createdAt
    };

    public Order ToModel(IReadOnlyList<FixedItem> items) => new(ItemName, StatusText, Total)
    {
        Number = Number.StartsWith(SeedPrefix) ? "" : Number,
        Items = items, Shipping = Shipping, Details = Details
    };
}

[Table("order_items")]
public class OrderItemRow : BaseModel
{
    [PrimaryKey("id", false)] public long Id { get; set; }
    [Column("order_number")] public string OrderNumber { get; set; } = "";
    [Column("item_id")] public string ItemId { get; set; } = "";
    [Column("price")] public int Price { get; set; }
}

// ---------------------------------------------------------------------------
// The one class that talks to Supabase.
// ---------------------------------------------------------------------------

public sealed class SupabaseDb
{
    private readonly Supabase.Client _client;
    public SupabaseDb(Supabase.Client client) => _client = client;

    // ---- First-run seeding ---------------------------------------------------

    /// <summary>Fills the tables from SeedData if they are empty (first run only).</summary>
    public async Task SeedIfEmptyAsync()
    {
        var hasAuctions = (await _client.From<AuctionRow>().Limit(1).Get()).Models.Count > 0;
        var hasItems = (await _client.From<FixedItemRow>().Limit(1).Get()).Models.Count > 0;
        if (hasAuctions || hasItems) return;

        await _client.From<FixedItemRow>().Insert(SeedData.CreateFixedItems().Select(FixedItemRow.From).ToList());

        var auctions = SeedData.CreateAuctions();
        await _client.From<AuctionRow>().Insert(auctions.Select(AuctionRow.From).ToList());

        // History is newest-first in the app; insert oldest-first so ids ascend with time.
        var bids = auctions.SelectMany(a => Enumerable.Reverse(a.History).Select(h => new BidRow
        {
            AuctionId = a.Id, Who = h.Who, Amount = h.Amount, AgoLabel = h.Ago
        })).ToList();
        if (bids.Count > 0) await _client.From<BidRow>().Insert(bids);

        var now = DateTimeOffset.UtcNow;
        var orders = SeedData.Orders.Select((o, i) => OrderRow.From(o, now.AddMinutes(-i), i)).ToList();
        if (orders.Count > 0) await _client.From<OrderRow>().Insert(orders);
    }

    // ---- READ ------------------------------------------------------------------

    /// <summary>2 requests total (auctions + all their bids), not 1 + N.</summary>
    public async Task<List<Auction>> LoadAuctionsAsync()
    {
        var auctions = (await _client.From<AuctionRow>()
            .Order(a => a.EndsAtUtc, Ordering.Ascending)
            .Get()).Models;

        var bids = (await _client.From<BidRow>()
            .Order(b => b.Id, Ordering.Descending)       // newest first
            .Get()).Models;

        var byAuction = bids.GroupBy(b => b.AuctionId)
                            .ToDictionary(g => g.Key, g => g.Select(b => b.ToModel()).ToList());

        return auctions.Select(a => a.ToModel(byAuction.GetValueOrDefault(a.Id) ?? new())).ToList();
    }

    /// <summary>The database filters out soft-deleted rows and caps the page size.</summary>
    public async Task<List<FixedItem>> LoadItemsAsync()
    {
        var res = await _client.From<FixedItemRow>()
            .Where(r => r.DeletedAt == null)             // habit 2: filter in the database
            .Order(r => r.CreatedAt, Ordering.Descending)
            .Order(r => r.Id, Ordering.Ascending)
            .Range(0, 199)                               // first 200
            .Get();
        return res.Models.Select(r => r.ToModel()).ToList();
    }

    /// <summary>3 requests total (orders, their lines, the items those lines point to).</summary>
    public async Task<List<Order>> LoadOrdersAsync()
    {
        var orders = (await _client.From<OrderRow>()
            .Order(o => o.CreatedAt, Ordering.Descending)
            .Get()).Models;

        var lines = (await _client.From<OrderItemRow>().Get()).Models;
        var itemIds = lines.Select(l => l.ItemId).Distinct().Cast<object>().ToList();

        // Sold items are soft-deleted, so do NOT filter on deleted_at here: order history still needs them.
        var items = new Dictionary<string, FixedItem>();
        if (itemIds.Count > 0)
        {
            var rows = (await _client.From<FixedItemRow>()
                .Filter("id", Operator.In, itemIds)
                .Get()).Models;
            items = rows.ToDictionary(r => r.Id, r => r.ToModel());
        }

        var linesByOrder = lines.GroupBy(l => l.OrderNumber).ToDictionary(g => g.Key, g => g.ToList());

        return orders.Select(o =>
        {
            var orderItems = (linesByOrder.GetValueOrDefault(o.Number) ?? new())
                .Where(l => items.ContainsKey(l.ItemId))
                .Select(l => items[l.ItemId] with { Price = l.Price })   // price at time of purchase
                .ToList();
            return o.ToModel(orderItems);
        }).ToList();
    }

    // ---- CREATE ------------------------------------------------------------------

    public Task AddItemAsync(FixedItem i) =>
        Safe(() => _client.From<FixedItemRow>().Insert(FixedItemRow.From(i)));

    public Task AddAuctionAsync(Auction a) =>
        Safe(() => _client.From<AuctionRow>().Insert(AuctionRow.From(a)));

    public Task AddOrderAsync(Order o) => Safe(async () =>
    {
        await _client.From<OrderRow>().Insert(OrderRow.From(o, DateTimeOffset.UtcNow));
        if (o.Items.Count > 0)
            await _client.From<OrderItemRow>().Insert(o.Items.Select(i => new OrderItemRow
            {
                OrderNumber = o.Number, ItemId = i.Id, Price = i.Price
            }).ToList());                                // one request for all lines
    });

    // ---- UPDATE (a bid: append a log row, then PATCH the auction) ---------------------

    public Task PlaceBidAsync(Auction a, BidEntry bid) => Safe(async () =>
    {
        await _client.From<BidRow>().Insert(new BidRow { AuctionId = a.Id, Who = bid.Who, Amount = bid.Amount });
        await _client.From<AuctionRow>()
            .Where(r => r.Id == a.Id)
            .Set(r => r.BidCount, a.BidCount)
            .Set(r => r.Leader, a.Leader.ToString())
            .Set(r => r.YourBid, a.YourBid)
            .Update();
    });

    // ---- DELETE (soft): one PATCH for all ids, not one per id --------------------------

    public Task SoftDeleteItemsAsync(IEnumerable<string> ids)
    {
        var list = ids.Cast<object>().ToList();
        return Safe(() => _client.From<FixedItemRow>()
            .Filter("id", Operator.In, list)
            .Set(r => r.DeletedAt, DateTime.UtcNow)
            .Update());
    }

    // Writes are fire-and-forget from the UI; log failures instead of crashing the page.
    private static async Task Safe(Func<Task> work)
    {
        try { await work(); }
        catch (Exception ex) { Console.Error.WriteLine($"Supabase write failed: {ex.Message}"); }
    }
}
