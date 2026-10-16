namespace MidtermProject.Models;

/// <summary>A live auction listing together with its bid history.</summary>
public class Auction
{
    public const int BidIncrement = 10;
    private static readonly TimeSpan UrgentThreshold = TimeSpan.FromHours(1);

    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Brand { get; init; }
    public required string Condition { get; init; }
    public required string Img { get; init; }
    public required int StartingBid { get; init; }
    public required DateTime EndsAtUtc { get; init; }

    public bool IsYours { get; init; }
    public ConditionReport? Report { get; init; }
    public int BidCount { get; set; }
    public BidLeader Leader { get; set; }
    public int? YourBid { get; set; }

    /// <summary>Newest bid first.</summary>
    public List<BidEntry> History { get; init; } = new();

    public string FullName => $"{Brand} {Name}";
    public string LotNumber => Id.ToUpperInvariant();
    public int CurrentBid => History.Count > 0 ? History[0].Amount : StartingBid;
    public int MinimumNextBid => CurrentBid + BidIncrement;

    public BidStatus Status =>
        Leader == BidLeader.You ? BidStatus.Highest
        : YourBid.HasValue ? BidStatus.Outbid
        : BidStatus.None;

    public TimeSpan TimeLeft
    {
        get
        {
            var left = EndsAtUtc - DateTime.UtcNow;
            return left < TimeSpan.Zero ? TimeSpan.Zero : left;
        }
    }

    public bool HasEnded => TimeLeft <= TimeSpan.Zero;
    public bool IsUrgent => TimeLeft < UrgentThreshold;
}
