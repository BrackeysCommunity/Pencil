namespace Pencil.Configuration;

/// <summary>
///     Represents a guild configuration.
/// </summary>
internal sealed class GuildConfiguration
{
    /// <summary>
    ///     Gets or sets the array of filtered expressions.
    /// </summary>
    public string[] FilteredRegexes { get; set; } = {"nigg+(a+|e+r+)", "re+ta+rd(?!ant)"};
}
