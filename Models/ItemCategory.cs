namespace MidtermProject.Models;

/// <summary>Top-level groups shown as columns in the Shop menu.</summary>
public enum CategoryGroup
{
    Instruments,
    Audio,
    Gadgets,
    Other
}

/// <summary>Product categories for marketplace listings.</summary>
public enum ItemCategory
{
    // Instruments
    Guitars,
    Basses,
    Drums,
    Percussion,
    Keyboards,
    Synths,
    Strings,
    Brass,
    Woodwinds,

    // Audio & studio
    Amps,
    Microphones,
    Mixers,
    Monitors,
    Pedals,
    Headphones,

    // Gadgets
    Tuners,
    DjGear,
    Interfaces,
    Controllers,

    // Others
    Cables,
    Accessories,
    Cases,
    Parts
}

public static class ItemCategoryExtensions
{
    public static string DisplayName(this ItemCategory category) => category switch
    {
        ItemCategory.Guitars => "Guitars",
        ItemCategory.Basses => "Basses",
        ItemCategory.Drums => "Drums & Kits",
        ItemCategory.Percussion => "Cymbals & Percussion",
        ItemCategory.Keyboards => "Keyboards & Pianos",
        ItemCategory.Synths => "Synths & Samplers",
        ItemCategory.Strings => "Violins & Orchestral Strings",
        ItemCategory.Brass => "Brass",
        ItemCategory.Woodwinds => "Woodwinds",
        ItemCategory.Amps => "Amps",
        ItemCategory.Microphones => "Microphones",
        ItemCategory.Mixers => "Mixers",
        ItemCategory.Monitors => "Studio Monitors",
        ItemCategory.Pedals => "Pedals & Effects",
        ItemCategory.Headphones => "Headphones",
        ItemCategory.Tuners => "Tuners & Metronomes",
        ItemCategory.DjGear => "DJ Gear",
        ItemCategory.Interfaces => "Audio Interfaces",
        ItemCategory.Controllers => "MIDI Controllers",
        ItemCategory.Cables => "Cables",
        ItemCategory.Accessories => "Stands & Accessories",
        ItemCategory.Cases => "Cases & Bags",
        ItemCategory.Parts => "Parts & Repair",
        _ => category.ToString()
    };

    public static CategoryGroup Group(this ItemCategory category) => category switch
    {
        ItemCategory.Guitars or ItemCategory.Basses or ItemCategory.Drums or ItemCategory.Percussion
            or ItemCategory.Keyboards or ItemCategory.Synths or ItemCategory.Strings
            or ItemCategory.Brass or ItemCategory.Woodwinds => CategoryGroup.Instruments,
        ItemCategory.Amps or ItemCategory.Microphones or ItemCategory.Mixers
            or ItemCategory.Monitors or ItemCategory.Pedals or ItemCategory.Headphones => CategoryGroup.Audio,
        ItemCategory.Tuners or ItemCategory.DjGear or ItemCategory.Interfaces
            or ItemCategory.Controllers => CategoryGroup.Gadgets,
        _ => CategoryGroup.Other
    };

    public static string DisplayName(this CategoryGroup group) => group switch
    {
        CategoryGroup.Instruments => "Instruments",
        CategoryGroup.Audio => "Audio & Studio",
        CategoryGroup.Gadgets => "Gadgets",
        _ => "Others"
    };

    public static IEnumerable<ItemCategory> InGroup(CategoryGroup group) =>
        Enum.GetValues<ItemCategory>().Where(c => c.Group() == group);
}
