
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Data;

/// <summary>
/// Wraps an enum to be serializes appropriately.
/// </summary>
/// <typeparam name="T">The enum type to be serialized.</typeparam>
[Serializable]
public class SerializableEnum<T> where T : Enum
{
    /// <summary>
    /// A value associated with the current instance.
    /// </summary>
    protected T value = default!;

    /// <summary>
    /// Gets or sets the string representation of the value.
    /// </summary>
    public string Value
    {
        get => this.value.ToString<SerializationTextAttribute>();
        set => this.value = value.ToEnum<T, SerializationTextAttribute>();
    }

    /// <summary>
    /// Initializes a new instanece of the <see cref="SerializableEnum{T}"/> class.
    /// </summary>
    public SerializableEnum() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializableEnum{T}"/> class with the inital value.
    /// </summary>
    /// <param name="value">The initial value.</param>
    public SerializableEnum(T value)
    {
        this.value = value;
    } // ctor (value)

    /// <summary>
    /// Casts a <typeparamref name="T"/> into its corresponding <see cref="SerializableEnum{T}"/>.
    /// </summary>
    /// <param name="value">An instance of the <typeparamref name="T"/> to be casted.</param>
    public static implicit operator SerializableEnum<T>(T value)
        => new(value);

    /// <summary>
    /// Casts a <see cref="SerializableEnum{T}"/> into its corresponding <typeparamref name="T"/> instance.
    /// </summary>
    /// <param name="value">An instance of the <see cref="SerializableEnum{T}"/> to be casted.</param>
    public static implicit operator T(SerializableEnum<T> value)
        => value.value;
} // public class SerializableEnum<T> where T : Enum
