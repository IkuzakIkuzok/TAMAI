
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
    [SerializationText("not-specified")]
    NotSpecified,

    /// <summary>
    /// Microsecond-TAS (usTAS).
    /// </summary>
    [SerializationText("us-tas")]
    MicroSecond,

    /// <summary>
    /// Femtosecond-TAS (fsTAS).
    /// </summary>
    [SerializationText("fs-tas")]
    FemtoSecond,
} // public enum TasTypes
