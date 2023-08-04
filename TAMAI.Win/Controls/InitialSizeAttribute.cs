
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win.Controls;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
internal sealed class InitialSizeAttribute : Attribute
{
    internal int Width { get; }
    internal int Height { get; }

    internal InitialSizeAttribute(int width, int height)
    {
        this.Width = width;
        this.Height = height;
    } // ctor (int, int)
} // internal sealed class InitialSizeAttribute : Attribute
