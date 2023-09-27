
// (c) 2023 Kazuki KOHZUKI

using System.Reflection;
using System.Xml.Serialization;

namespace TAMAI;

/// <summary>
/// Represents a scientific value, with appropriate representation.
/// </summary>
/// <typeparam name="T">The base type of the value.</typeparam>
[Serializable]
public sealed class ScientificValue<T> where T : IPhysicalQuantity<T>
{
    private static readonly char[] PREFIX = [
        'Q', 'R', 'Y', 'Z', 'E', 'P', 'T', 'G', 'M', 'k', '\0', 'm', 'u', 'n', 'p', 'f', 'a', 'z', 'y', 'r', 'q'
    ];
    private const int CENTER = 10;

    static ScientificValue()
    {
        var attr = Attribute.GetCustomAttributes(typeof(T), false)
            .Where(attr => attr is PhysicalQuantityAttribute)
            .FirstOrDefault();

        if (attr is not PhysicalQuantityAttribute unitAttr)
            throw new TypeInitializationException(typeof(ScientificValue<T>).FullName, null);

        var name = unitAttr.ValueName;
        var memberInfo = typeof(T).GetMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0];
        GetValue = memberInfo.MemberType switch
        {
            MemberTypes.Field    => (obj) => (double)(((FieldInfo)memberInfo).GetValue(obj) ?? .0),
            MemberTypes.Property => (obj) => (double)(((PropertyInfo)memberInfo).GetValue(obj) ?? .0),
            _ => throw new TypeInitializationException(typeof(ScientificValue<T>).FullName, null),
        };

        unit = unitAttr.Unit;
        acceptSIPrefix = unitAttr.AcceptSIPrefix;
        defaultValue = unitAttr.DefaultValue;
    } // cctor ()

    private static readonly Func<object?, double> GetValue;
    private static readonly string unit = string.Empty;
    private static readonly bool acceptSIPrefix = true;
    private static readonly double defaultValue = 0;
    
    /// <summary>
    /// Gets or set the value.
    /// </summary>
    [XmlIgnore]
    public T? Value { get; private set; }

    /// <summary>
    /// Gets the current value as an instance of double.
    /// </summary>
    [XmlIgnore]
    public double ValueAsDouble => GetValue(this.Value);

    /// <summary>
    /// Gets or sets the string representation of a scientific value associated with the current instance.
    /// </summary>
    [XmlText]
    public string Text
    {
        get
        {
            var value = GetValue(this.Value);
            var norm = Math.Round(Normalize(value, out var p), 6);
            return $"{norm} {(p == 0 ? string.Empty : p.ToString())}{unit}";
        }
        set
        {
            var data = value.Split(' ');
            var v = double.Parse(data[0]);
            var p = data[1][..^unit.Length];
            var prefix = string.IsNullOrEmpty(p) ? '\0' : p[0];
            var n = CENTER - Array.IndexOf(PREFIX, prefix);
            this.Value = T.FromDouble(v * Math.Pow(10, n * 3));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScientificValue{T}"/> class.
    /// </summary>
    public ScientificValue()
    {
        this.Value = T.FromDouble(defaultValue);
    } // ctor ()

    /// <summary>
    /// Initializes a new instance of the <see cref="ScientificValue{T}"/> class
    /// with its initial value.
    /// </summary>
    /// <param name="value">The initial value.</param>
    public ScientificValue(T value)
    {
        this.Value = value;
    } // ctor (T)

    /// <summary>
    /// Initializes a new instance of the <see cref="ScientificValue{T}"/> class
    /// with its initial value represented as a string.
    /// </summary>
    /// <param name="s">A string representation of the initial value.</param>
    public ScientificValue(string s)
    {
        this.Text = s;
    } // ctor (string)

    /// <summary>
    /// Casts an instance of <see cref="ScientificValue{T}"/> into its corresponding <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">The value to be casted.</param>
    public static implicit operator T?(ScientificValue<T> value)
        => value.Value;

    /// <summary>
    /// Casts an instance of <typeparamref name="T"/> into its corresponding <see cref="ScientificValue{T}"/>.
    /// </summary>
    /// <param name="value">The value to be casted.</param>
    public static implicit operator ScientificValue<T>(T value)
        => new(value);

    /// <inheritdoc/>
    override public string ToString()
        => this.Text;

    /// <summary>
    /// Normalizes a value with a representation with appropriate SI prefix.
    /// </summary>
    /// <param name="value">The value to be normalized.</param>
    /// <param name="prefix">The SI prefix.</param>
    /// <returns>The normalized value.</returns>
    private static double Normalize(double value, out char prefix)
    {
        prefix = '\0';
        if (!acceptSIPrefix) return value;

        var n = 0;
        while (Math.Abs(value) > 1_000)
        {
            if (n >= 10) break;
            value /= 1_000;
            ++n;
        }
        while (Math.Abs(value) < 1)
        {
            if (n <= -10) break;
            value *= 1_000;
            --n;
        }

        prefix = PREFIX[CENTER - n];
        return value;
    } // private static double Normalize (double, out char)
} // public sealed class ScientificValue<T> : where T : IPhysicalQuantity<T>
