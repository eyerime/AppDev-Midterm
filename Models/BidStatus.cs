namespace MidtermProject.Models;

/// <summary>The signed-in user's standing in an auction.</summary>
public enum BidStatus
{
    None,
    Highest,
    Outbid
}

/// <summary>Who currently holds the top bid.</summary>
public enum BidLeader
{
    Other,
    You
}
