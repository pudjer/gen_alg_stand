namespace Parameters;

/// <summary>
/// Immutable description of a single tunable knob: how it is displayed, how it is parsed and
/// what values are acceptable.
/// </summary>
/// <param name="Key">Machine name used in configuration dictionaries.</param>
/// <param name="DisplayName">Human readable label.</param>
/// <param name="Kind">Editor and validation kind.</param>
/// <param name="Default">Default value, always valid for this spec.</param>
/// <param name="Description">Optional help text.</param>
/// <param name="Min">Inclusive lower bound for numeric kinds.</param>
/// <param name="Max">Inclusive upper bound for numeric kinds.</param>
/// <param name="Step">Suggested editor increment for numeric kinds.</param>
/// <param name="Options">Allowed values for <see cref="ParameterKind.Choice"/>.</param>
public sealed record ParameterSpec(
    string Key,
    string DisplayName,
    ParameterKind Kind,
    string Default,
    string? Description = null,
    double? Min = null,
    double? Max = null,
    double? Step = null,
    IReadOnlyList<string>? Options = null)
{
    /// <summary>Gets a value indicating whether this spec describes a numeric parameter.</summary>
    public bool IsNumeric => Kind is ParameterKind.Int or ParameterKind.Double;
}
