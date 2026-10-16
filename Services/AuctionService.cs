using MidtermProject.Data;
using MidtermProject.Helpers;
using MidtermProject.Models;

namespace MidtermProject.Services;

/// <summary>
/// Holds live auction state and ticks once per second so countdowns re-render.
/// Registered as a singleton; components subscribe to <see cref="Changed"/>.
/// </summary>
public sealed class AuctionService : IDisposable
{
    private readonly List<Auction> _auctions = SeedData.CreateAuctions();
    private readonly Timer _timer;

    public AuctionService()
    {
        _timer = new Timer(_ => Changed?.Invoke(), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    /// <summary>Raised every second and whenever a bid is placed.</summary>
    public event Action? Changed;

    public IReadOnlyList<Auction> Auctions => _auctions;

    public IEnumerable<Auction> GetYourAuctions() => _auctions.Where(a => a.IsYours);

    public void Add(Auction auction)
    {
        _auctions.Insert(0, auction);
        Changed?.Invoke();
    }

    public Auction? Get(string id) =>
        _auctions.FirstOrDefault(a => string.Equals(a.Id, id, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Auction> Sort(AuctionSort sort) => sort switch
    {
        AuctionSort.LowestBid => _auctions.OrderBy(a => a.CurrentBid),
        AuctionSort.HighestBid => _auctions.OrderByDescending(a => a.CurrentBid),
        _ => _auctions.OrderBy(a => a.EndsAtUtc),
    };

    public BidResult PlaceBid(string id, int amount)
    {
        var auction = Get(id);
        if (auction is null)
            return new BidResult(false, "Auction not found");
        if (auction.HasEnded)
            return new BidResult(false, "This auction has ended");
        if (amount < auction.MinimumNextBid)
            return new BidResult(false, $"Minimum bid is {Format.Money(auction.MinimumNextBid)}");

        auction.History.Insert(0, new BidEntry(BidEntry.YouLabel, amount, "just now"));
        auction.BidCount++;
        auction.Leader = BidLeader.You;
        auction.YourBid = amount;

        Changed?.Invoke();
        return new BidResult(true, $"You're the highest bidder at {Format.Money(amount)}");
    }

    public void Dispose() => _timer.Dispose();
}

public enum AuctionSort
{
    EndingSoonest,
    LowestBid,
    HighestBid
}
