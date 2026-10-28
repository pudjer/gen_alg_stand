namespace Parameters;

/// <summary>Describes how a parameter value is edited and validated.</summary>
public enum ParameterKind
{
    /// <summary>A 32-bit signed integer.</summary>
    Int,

    /// <summary>A double-precision floating point number.</summary>
    Double,

    /// <summary>A boolean flag.</summary>
    Bool,

    /// <summary>Free-form text.</summary>
    Text,

    /// <summary>One of a fixed set of options.</summary>
    Choice,
}
