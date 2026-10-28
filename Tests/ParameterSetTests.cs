using Parameters;

namespace Tests;

public sealed class ParameterSetTests
{
    private static IReadOnlyList<ParameterSpec> Specs =>
    [
        new("count", "Count", ParameterKind.Int, "7", Min: 0, Max: 10),
        new("rate", "Rate", ParameterKind.Double, "0.25", Min: 0, Max: 1),
        new("enabled", "Enabled", ParameterKind.Bool, "true"),
        new("label", "Label", ParameterKind.Text, "hello"),
        new("mode", "Mode", ParameterKind.Choice, "fast", Options: ["fast", "slow"]),
    ];

    [Fact]
    public void FromDefaults_RoundTripsEveryKind()
    {
        ParameterSet set = ParameterSet.FromDefaults(Specs);

        Assert.Equal(7, set.GetInt("count"));
        Assert.Equal(0.25, set.GetDouble("rate"), 12);
        Assert.True(set.GetBool("enabled"));
        Assert.Equal("hello", set.GetText("label"));
        Assert.Equal("fast", set.GetText("mode"));
    }

    [Fact]
    public void Create_UnknownKey_Throws()
    {
        var raw = new Dictionary<string, string> { ["count"] = "3", ["doesNotExist"] = "1" };

        var exception = Assert.Throws<ArgumentException>(() => ParameterSet.Create(Specs, raw));
        Assert.Contains("doesNotExist", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Create_OutOfRangeInt_Throws()
    {
        var raw = new Dictionary<string, string> { ["count"] = "42" };

        Assert.Throws<ArgumentException>(() => ParameterSet.Create(Specs, raw));
    }

    [Fact]
    public void Create_OutOfRangeDouble_Throws()
    {
        var raw = new Dictionary<string, string> { ["rate"] = "1.5" };

        Assert.Throws<ArgumentException>(() => ParameterSet.Create(Specs, raw));
    }

    [Fact]
    public void Create_ChoiceOutsideOptions_Throws()
    {
        var raw = new Dictionary<string, string> { ["mode"] = "medium" };

        Assert.Throws<ArgumentException>(() => ParameterSet.Create(Specs, raw));
    }

    [Fact]
    public void Create_MissingKey_FallsBackToDefault()
    {
        var raw = new Dictionary<string, string> { ["count"] = "3" };

        ParameterSet set = ParameterSet.Create(Specs, raw);

        Assert.Equal(3, set.GetInt("count"));
        Assert.Equal(0.25, set.GetDouble("rate"), 12);
        Assert.True(set.GetBool("enabled"));
        Assert.Equal("hello", set.GetText("label"));
        Assert.Equal("fast", set.GetText("mode"));
    }

    [Fact]
    public void With_ReturnsNewInstanceAndLeavesOriginalUnchanged()
    {
        ParameterSet original = ParameterSet.FromDefaults(Specs);
        ParameterSpec count = Specs[0];

        ParameterSet updated = original.With(count, "9");

        Assert.NotSame(original, updated);
        Assert.Equal(7, original.GetInt("count"));
        Assert.Equal(9, updated.GetInt("count"));
        Assert.Equal("hello", updated.GetText("label"));
        Assert.NotEqual(original, updated);
    }

    [Fact]
    public void With_InvalidValue_ThrowsAndLeavesOriginalUnchanged()
    {
        ParameterSet original = ParameterSet.FromDefaults(Specs);

        Assert.Throws<ArgumentException>(() => original.With(Specs[0], "999"));
        Assert.Equal(7, original.GetInt("count"));
    }
}