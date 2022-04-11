using System.Reflection;
using BrackeysBot.API.Plugins;
using NLog;

namespace Pencil;

/// <summary>
///     Represents a class which serves as an API adapter to Hawkeye.
/// </summary>
/// <remarks>
///     Pencil has a soft dependency of Hawkeye. Installing Hawkeye is not a hard requirement, but Pencil will use it if it can.
/// </remarks>
internal sealed class HawkeyeAdapter
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly IPluginManager _pluginManager;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HawkeyeAdapter" /> class.
    /// </summary>
    /// <param name="pluginManager">The plugin manager.</param>
    public HawkeyeAdapter(IPluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    /// <summary>
    ///     Returns a value indicating whether the specified input contains a filtered expression.
    /// </summary>
    /// <param name="input">The input to validate.</param>
    /// <returns>
    ///     <see langword="true" /> if the input contains a filtered expression; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    ///     If Hawkeye is installed and loaded, implementation is delegated to it. Otherwise, <see langword="false" /> is returned
    ///     by default.
    /// </remarks>
    public bool ContainsFilteredExpression(string input)
    {
        if (!_pluginManager.TryGetPlugin("Hawkeye", out IPlugin? plugin) || !_pluginManager.IsPluginEnabled(plugin))
            return false;

        Logger.Debug("Hawkeye detected, delegating ContainsFilteredExpression call!");

        // reflection fuckery to avoid referencing Hawkeye directly
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        MethodInfo? method = plugin.GetType().GetMethod("ContainsFilteredExpression", bindingFlags);
        return method?.Invoke(plugin, new object?[] {input}) is true;
    }
}
