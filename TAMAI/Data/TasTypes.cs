
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Represents a TAS measurement type.
/// </summary>
public enum TasTypes
{
    /// <summary>
    /// Not specified.
    /// </summary>
    [SerializatoinText("not-specified")]
    NotSpecified,

    /// <summary>
    /// Microsecond-TAS (usTAS).
    /// </summary>
    [SerializatoinText("us-tas")]
    MicroSecond,

    /// <summary>
    /// Femtosecond-TAS (fsTAS).
    /// </summary>
    [SerializatoinText("fs-tas")]
    FemtoSecond,
} // public enum TasTypes
