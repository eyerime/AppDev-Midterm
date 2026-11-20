namespace MidtermProject.Models;

/// <summary>The (mock) signed-in member.</summary>
public record UserProfile(string Name, string Initials, int MemberSince, int GearDeals)
{
    public string Subtitle => $"Member since {MemberSince} · {GearDeals} gear deals";
}
