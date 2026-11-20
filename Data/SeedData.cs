using MidtermProject.Models;

namespace MidtermProject.Data;

/// <summary>In-memory demo data ported from the Figma design.</summary>
public static class SeedData
{
    public const string HeroImage =
        "https://images.unsplash.com/photo-1516924962500-2b4b3b99ea02?w=2000&fit=crop&auto=format&q=80";

    public static string PlaceholderImage => Photo("1516924962500-2b4b3b99ea02", 1200);

    private static string Photo(string id, int size = 900) =>
        $"https://images.unsplash.com/photo-{id}?w={size}&h={size}&fit=crop&auto=format&q=80";


    private static FunctionCheck Ok(string label, string? note = null) => new(label, true, note);

    private static FunctionCheck Issue(string label, string note) => new(label, false, note);

    private static ConditionReport ReportFor(string id) => Reports[id];

    /// <summary>Demo condition reports (invented for the prototype).</summary>
    private static readonly Dictionary<string, ConditionReport> Reports = new()
    {
        ["f1"] = new()
        {
            YearMade = 1994, OwnerCount = 2, Usage = UsageLevel.Regular, OriginalPrice = 1450,
            ReasonForSelling = "Upgrading to a vintage reissue.",
            Wear = { new("Back of neck", "Light finish wear from playing", WearSeverity.Minor), new("Lower bout", "Two small dings", WearSeverity.Minor), new("Pickguard", "Faint surface scratches", WearSeverity.Minor) },
            Checks = { Ok("Powers on / produces sound"), Ok("Electronics & controls tested", "All pickups tested; 5-way switch is clean"), Ok("Hardware & mechanics working"), Ok("Plays & tunes correctly", "Recent setup, low action") },
            Included = { "Gig bag", "Strap", "Allen keys" },
            Modifications = { "Tuners replaced with locking tuners" },
            SerialVerified = true,
        },
        ["f2"] = new()
        {
            YearMade = 2021, OwnerCount = 1, Usage = UsageLevel.Light, OriginalPrice = 899,
            ReasonForSelling = "Doesn't suit my playing style.",
            Wear = { new("Headstock", "Tiny scuff on the back", WearSeverity.Minor) },
            Checks = { Ok("Powers on / produces sound"), Ok("Electronics & controls tested"), Ok("Hardware & mechanics working"), Ok("Plays & tunes correctly") },
            Included = { "Original gig bag", "Spare strings", "Manual" },
            SerialVerified = true, HasReceipt = true,
        },
        ["f3"] = new()
        {
            YearMade = 2016, OwnerCount = 1, Usage = UsageLevel.Regular, OriginalPrice = 1300,
            ReasonForSelling = "Moving to a smaller flat.",
            Wear = { new("Rack tom", "Small chip in the wrap", WearSeverity.Minor), new("Hi-hat stand", "Some surface rust on the legs", WearSeverity.Moderate) },
            Checks = { Ok("Hardware & mechanics working", "All lugs and tension rods turn freely"), Ok("Plays & tunes correctly", "Heads are about a year old") },
            Included = { "Throne", "Kick pedal", "Drum key" },
            Modifications = { "Snare head replaced" },
            HasReceipt = true,
        },
        ["f4"] = new()
        {
            YearMade = 2019, OwnerCount = 1, Usage = UsageLevel.Regular, OriginalPrice = 1400,
            ReasonForSelling = "Switching to a weighted-key piano.",
            NoVisibleWear = true,
            Checks = { Ok("Powers on / produces sound"), Ok("Electronics & controls tested", "All 88 keys and 3 pedal inputs tested"), Ok("Hardware & mechanics working") },
            Included = { "Power supply", "Sustain pedal", "Music stand" },
            SerialVerified = true, HasReceipt = true,
        },
        ["f5"] = new()
        {
            YearMade = 2012, OwnerCount = 3, Usage = UsageLevel.Heavy, OriginalPrice = 650,
            ReasonForSelling = "Band broke up; clearing out gear.",
            Wear = { new("Body", "Belt-buckle rash on the back", WearSeverity.Moderate), new("Frets", "Moderate wear, playable", WearSeverity.Moderate), new("Output jack", "Slightly loose", WearSeverity.Moderate) },
            Checks = { Ok("Powers on / produces sound"), Ok("Hardware & mechanics working"), Issue("Electronics & controls tested", "Neck pickup crackles when the volume knob is turned") },
            Included = { "Hard case" },
            Modifications = { "Pickups swapped for aftermarket humbuckers", "Replaced bridge" },
        },
        ["f6"] = new()
        {
            YearMade = 2018, OwnerCount = 1, Usage = UsageLevel.Regular, OriginalPrice = 400,
            ReasonForSelling = "Own two snares; this one is surplus.",
            Wear = { new("Shell", "Light scratches", WearSeverity.Minor), new("Rim", "Slight tarnish", WearSeverity.Minor) },
            Checks = { Ok("Hardware & mechanics working"), Ok("Plays & tunes correctly", "Snare wires are in good shape") },
            Included = { "Snare bag" },
            HasReceipt = true,
        },
        ["f7"] = new()
        {
            YearMade = 2015, OwnerCount = 2, Usage = UsageLevel.Regular, OriginalPrice = 749,
            ReasonForSelling = "Going all-digital.",
            Wear = { new("Faders", "Slightly gritty on channel 11", WearSeverity.Moderate), new("Chassis", "Scuffs on the side panels", WearSeverity.Minor) },
            Checks = { Ok("Powers on / produces sound"), Ok("Electronics & controls tested", "All channels tested; 15 of 16 clean"), Issue("Hardware & mechanics working", "Channel 11 fader needs contact cleaner") },
            Included = { "Power cable", "Rack ears" },
        },
        ["f8"] = new()
        {
            YearMade = 2020, OwnerCount = 1, Usage = UsageLevel.Light, OriginalPrice = 900,
            ReasonForSelling = "Too loud for the apartment.",
            Wear = { new("Tolex", "One small tear on a corner", WearSeverity.Minor) },
            Checks = { Ok("Powers on / produces sound"), Ok("Electronics & controls tested"), Ok("Hardware & mechanics working"), Ok("Plays & tunes correctly", "Tubes tested; sounds great") },
            Included = { "Footswitch", "Power cable", "Cover" },
            SerialVerified = true, HasReceipt = true,
        },
        ["a1"] = new()
        {
            YearMade = 1978, OwnerCount = 3, Usage = UsageLevel.Heavy,
            ReasonForSelling = "Estate collection sale.",
            Wear = { new("Binding", "Checking and yellowing from age", WearSeverity.Moderate), new("Back", "Buckle wear", WearSeverity.Moderate), new("Headstock", "Small repaired crack", WearSeverity.Major) },
            Checks = { Ok("Powers on / produces sound"), Ok("Electronics & controls tested", "Pots and switch are working"), Ok("Hardware & mechanics working"), Ok("Plays & tunes correctly") },
            Included = { "Original hard case", "Case candy" },
            Modifications = { "Refretted in 1999", "Pickup covers replaced" },
            SerialVerified = true,
        },
        ["a2"] = new()
        {
            YearMade = 2022, OwnerCount = 1, Usage = UsageLevel.Light, OriginalPrice = 1100,
            ReasonForSelling = "Bought a second guitar in the same colour.",
            NoVisibleWear = true,
            Checks = { Ok("Powers on / produces sound"), Ok("Electronics & controls tested"), Ok("Hardware & mechanics working"), Ok("Plays & tunes correctly") },
            Included = { "Gig bag", "Tremolo arm", "Receipt" },
            SerialVerified = true, HasReceipt = true,
        },
        ["a3"] = new()
        {
            YearMade = 1982, OwnerCount = 2, Usage = UsageLevel.Regular,
            ReasonForSelling = "Studio clear-out.",
            Wear = { new("Wood end-cheeks", "Light scratches and dings", WearSeverity.Minor), new("Keybed", "Two keys slightly sticky", WearSeverity.Moderate) },
            Checks = { Ok("Powers on / produces sound"), Ok("Electronics & controls tested", "Recapped and recalibrated by a technician"), Issue("Hardware & mechanics working", "Two keys on the lower octave need a service") },
            Included = { "Power cable", "Service invoice" },
            Modifications = { "Power supply capacitors replaced", "MIDI retrofit installed" },
        },
        ["a4"] = new()
        {
            YearMade = 1976, OwnerCount = 4, Usage = UsageLevel.Heavy,
            ReasonForSelling = "Collection downsizing.",
            Wear = { new("Shells", "Fine scratches and a few chips in the acrylic", WearSeverity.Moderate), new("Floor tom", "Hairline crack near one lug", WearSeverity.Major) },
            Checks = { Ok("Hardware & mechanics working", "Original lugs, all functional"), Ok("Plays & tunes correctly") },
            Included = { "Cases for each drum" },
            Modifications = { "Newer heads", "Replacement bass drum hoops" },
        },
        ["a5"] = new()
        {
            YearMade = 1975, OwnerCount = 3, Usage = UsageLevel.Heavy,
            ReasonForSelling = "Moving countries.",
            Wear = { new("Tolex", "Scuffed on the lid and corners", WearSeverity.Moderate) },
            Checks = { Ok("Powers on / produces sound"), Ok("Electronics & controls tested", "Tines voiced and checked"), Issue("Hardware & mechanics working", "Sustain pedal is not included; one leg is stiff") },
            Included = { "Legs", "Lid" },
            Modifications = { "Replaced several hammer tips" },
        },
        ["a6"] = new()
        {
            YearMade = 2017, OwnerCount = 1, Usage = UsageLevel.Regular, OriginalPrice = 480,
            ReasonForSelling = "Switched to a larger ride.",
            Wear = { new("Surface", "Stick marks and light patina", WearSeverity.Minor) },
            Checks = { Ok("Powers on / produces sound", "No cracks or keyholing"), Ok("Plays & tunes correctly") },
            Included = { "Cymbal bag" },
            SerialVerified = false, HasReceipt = true,
        },
    };

    public static UserProfile User { get; } = new("Maya Kovacs", "MK", 2023, 12);

    public static IReadOnlyList<Order> Orders { get; } = new List<Order>
    {
        new("Maple Snare 14\"", "Delivered Oct 2", 260),
        new("Combo Amp 1x12", "Shipped Oct 12", 680),
    };

    public static IReadOnlyList<FixedItem> CreateFixedItems() => new List<FixedItem>
    {
        new("f1", "Stratocaster 1994", "Fender", ItemCategory.Guitars, "Very Good", 1180, Photo("1607560105214-0aaa5f8fcba4")) { Report = ReportFor("f1") },
        new("f2", "Teal Offset Jazzmaster", "Squier", ItemCategory.Guitars, "Excellent", 640, Photo("1564186763535-ebb21ef5277f")) { Report = ReportFor("f2") },
        new("f3", "Birch Studio Kit 5pc", "Pearl", ItemCategory.Drums, "Good", 790, Photo("1461784121038-f088ca1e7714")) { Report = ReportFor("f3") },
        new("f4", "Stage Piano 88", "Roland", ItemCategory.Keyboards, "Excellent", 920, Photo("1598653222000-6b7b7a552625")) { Report = ReportFor("f4") },
        new("f5", "Single-Cut Black Beauty", "Epiphone", ItemCategory.Guitars, "Good", 520, Photo("1550291652-6ea9114a47b1")) { Report = ReportFor("f5") },
        new("f6", "Maple Snare 14\"", "Ludwig", ItemCategory.Drums, "Very Good", 260, Photo("1519892300165-cb5542fb47c7")) { Report = ReportFor("f6") },
        new("f7", "16-Ch Analog Mixer", "Mackie", ItemCategory.Mixers, "Good", 410, Photo("1581028619469-8207e7185bd3")) { Report = ReportFor("f7") },
        new("f8", "Combo Amp 1x12", "Orange", ItemCategory.Amps, "Very Good", 680, Photo("1550985616-10810253b84d")) { Report = ReportFor("f8") },
    };

    /// <summary>Creates fresh auctions whose end times are relative to "now".</summary>
    public static List<Auction> CreateAuctions()
    {
        var now = DateTime.UtcNow;

        return new List<Auction>
        {
            new()
            {
                Id = "a1", Report = ReportFor("a1"), Name = "Les Paul Custom 1978", Brand = "Gibson", Condition = "Vintage",
                Img = Photo("1516924962500-2b4b3b99ea02", 1200), StartingBid = 1800,
                EndsAtUtc = now + new TimeSpan(2, 14, 59), BidCount = 23,
                Leader = BidLeader.You, YourBid = 2450,
                History =
                {
                    new("You", 2450, "4 min ago"),
                    new("User384…", 2400, "11 min ago"),
                    new("User912…", 2300, "26 min ago"),
                    new("User077…", 2150, "1 hr ago"),
                },
            },
            new()
            {
                Id = "a2", Report = ReportFor("a2"), Name = "Purple Haze Superstrat", Brand = "Ibanez", Condition = "Excellent",
                Img = Photo("1591365437381-2db81d59f1e0", 1200), StartingBid = 300,
                EndsAtUtc = now + new TimeSpan(0, 47, 12), BidCount = 14,
                Leader = BidLeader.Other, YourBid = 780,
                History =
                {
                    new("User551…", 820, "2 min ago"),
                    new("You", 780, "9 min ago"),
                    new("User204…", 700, "31 min ago"),
                },
            },
            new()
            {
                Id = "a3", Report = ReportFor("a3"), Name = "Poly-Six Analog Synth", Brand = "Korg", Condition = "Restored",
                Img = Photo("1733900606117-2522d8ed3a7b", 1200), StartingBid = 900,
                EndsAtUtc = now + new TimeSpan(5, 3, 8), BidCount = 31,
                Leader = BidLeader.Other,
                History =
                {
                    new("User618…", 1950, "1 min ago"),
                    new("User384…", 1900, "6 min ago"),
                    new("User190…", 1800, "18 min ago"),
                },
            },
            new()
            {
                Id = "a4", Report = ReportFor("a4"), Name = "Vistalite Kit 1976", Brand = "Ludwig", Condition = "Collector",
                Img = Photo("1708961465136-e24550f3acd5", 1200), StartingBid = 1500,
                EndsAtUtc = now + new TimeSpan(26, 40, 0), BidCount = 9,
                Leader = BidLeader.Other,
                History =
                {
                    new("User802…", 3100, "14 min ago"),
                    new("User133…", 2900, "2 hr ago"),
                },
            },
            new()
            {
                Id = "a5", Report = ReportFor("a5"), Name = "Rhodes Mark I Suitcase", Brand = "Fender Rhodes", Condition = "Good",
                Img = Photo("1611737833457-7fb3d06e8c36", 1200), StartingBid = 700,
                EndsAtUtc = now + new TimeSpan(0, 9, 41), BidCount = 40,
                Leader = BidLeader.Other, YourBid = 1650,
                History =
                {
                    new("User450…", 1700, "40 sec ago"),
                    new("You", 1650, "3 min ago"),
                    new("User921…", 1600, "8 min ago"),
                },
            },
            new()
            {
                Id = "a6", Report = ReportFor("a6"), Name = "Zildjian K Ride 22\"", Brand = "Zildjian", Condition = "Very Good",
                Img = Photo("1543443258-92b04ad5ec6b", 1200), StartingBid = 120,
                EndsAtUtc = now + new TimeSpan(3, 30, 0), BidCount = 6,
                Leader = BidLeader.Other,
                History =
                {
                    new("User066…", 245, "22 min ago"),
                    new("User312…", 220, "1 hr ago"),
                },
            },
        };
    }
}
