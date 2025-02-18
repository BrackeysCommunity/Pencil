using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pencil.Data;
using SixLabors.ImageSharp;
using X10D.Collections;

namespace Pencil.Services;

internal sealed partial class ColorService : BackgroundService
{
    private const string ColorsUrl = "https://raw.githubusercontent.com/joshbeckman/thecolorapi/" +
                                     "refs/heads/master/static/colorNames.json";

    private static readonly Regex SpaceRegex = GetSpaceRegex();
    private static readonly Regex HexRegex = GetHexRegex();
    private static readonly Regex RgbRegex = GetRgbRegex();
    private static readonly Regex HslRegex = GetHslRegex();
    private static readonly Regex CmykRegex = GetCmykRegex();
    private static readonly HttpClient HttpClient = new();
    private readonly Dictionary<string, Color> _predefinedColors = new(StringComparer.OrdinalIgnoreCase);

    private readonly ILogger<ColorService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ColorService" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public ColorService(ILogger<ColorService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Returns the color information for the given query.
    /// </summary>
    /// <param name="query">The query to search for.</param>
    /// <returns>The color information.</returns>
    public async Task<Response?> GetColorInformation(Dictionary<string, string> query)
    {
        var uri = $"https://www.thecolorapi.com/id?{query.ToGetParameters()}";
        await using Stream stream = await HttpClient.GetStreamAsync(uri);
        var response = JsonSerializer.Deserialize<Response>(stream);
        return response;
    }

    /// <summary>
    ///     Returns a value indicating whether the input is a named color.
    /// </summary>
    /// <param name="color">The color to check.</param>
    /// <param name="result">
    ///     When this method returns, contains the color if the input is a named color; otherwise, no specific value.
    /// </param>
    /// <returns><see langword="true" /> if the input is a named color; otherwise, <see langword="false" />.</returns>
    public bool IsNamedColor(string color, out Color result)
    {
        color = SpaceRegex.Replace(color, string.Empty);
        return _predefinedColors.TryGetValue(color, out result);
    }

    /// <summary>
    ///     Returns a value indicating whether the input is a hexadecimal color.
    /// </summary>
    /// <param name="input">The input to check.</param>
    /// <returns><see langword="true" /> if the input is a hexadecimal color; otherwise, <see langword="false" />.</returns>
    public bool IsHexColor(string input)
    {
        return HexRegex.IsMatch(input);
    }

    /// <summary>
    ///     Returns a value indicating whether the input is an RGB color.
    /// </summary>
    /// <param name="input">The input to check.</param>
    /// <returns><see langword="true" /> if the input is an RGB color; otherwise, <see langword="false" />.</returns>
    public bool IsRgbColor(string input)
    {
        return RgbRegex.IsMatch(input);
    }

    /// <summary>
    ///     Returns a value indicating whether the input is an HSL color.
    /// </summary>
    /// <param name="input">The input to check.</param>
    /// <returns><see langword="true" /> if the input is an HSL color; otherwise, <see langword="false" />.</returns>
    public bool IsHslColor(string input)
    {
        return HslRegex.IsMatch(input);
    }

    /// <summary>
    ///     Returns a value indicating whether the input is a CYMK color.
    /// </summary>
    /// <param name="input">The input to check.</param>
    /// <returns><see langword="true" /> if the input is a CMYK color; otherwise, <see langword="false" />.</returns>
    public bool IsCmykColor(string input)
    {
        return CmykRegex.IsMatch(input);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await LoadPredefinedColorsAsync();
    }

    private async Task LoadPredefinedColorsAsync()
    {
        _logger.LogInformation("Loading predefined colors");
        await using Stream stream = await HttpClient.GetStreamAsync(ColorsUrl);
        var response = await JsonSerializer.DeserializeAsync<Dictionary<string, ColorNameResponseSchema[]>>(stream);

        if (response is not null && response.TryGetValue("colors", out ColorNameResponseSchema[]? colors))
        {
            _logger.LogInformation("Fetched {Count} predefined colors", colors.Length);
            foreach (ColorNameResponseSchema color in colors)
            {
                string name = SpaceRegex.Replace(color.Name, string.Empty);
                _predefinedColors[name] = Color.FromRgb((byte)color.R, (byte)color.G, (byte)color.B);
            }
        }
        else
        {
            _logger.LogInformation("Failed to fetch predefined colors, using built-in colors");
            foreach (FieldInfo fieldInfo in typeof(Color)
                         .GetFields(BindingFlags.Public | BindingFlags.Static)
                         .Where(f => f.FieldType == typeof(Color)))
            {
                _predefinedColors[fieldInfo.Name] = (Color)fieldInfo.GetValue(null)!;
            }
        }

        _logger.LogInformation("Loaded {Count} predefined colors", _predefinedColors.Count);
    }

    [GeneratedRegex("\\s*")]
    private static partial Regex GetSpaceRegex();

    [GeneratedRegex("^#?(?:[0-9a-fA-F]{3}){1,2}$", RegexOptions.Compiled)]
    private static partial Regex GetHexRegex();

    [GeneratedRegex(@"^rgb\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)$", RegexOptions.Compiled)]
    private static partial Regex GetRgbRegex();

    [GeneratedRegex(@"^hsl\((\d{1,3}),\s*(\d{1,3})%,\s*(\d{1,3})%\)$", RegexOptions.Compiled)]
    private static partial Regex GetHslRegex();

    [GeneratedRegex(@"^cmyk\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)$", RegexOptions.Compiled)]
    private static partial Regex GetCmykRegex();
}
