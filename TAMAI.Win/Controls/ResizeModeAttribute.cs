
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win.Controls;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
internal sealed class ResizeModeAttribute : Attribute
{
    internal ResizeType ResizeType { get; }

    internal ResizeType HorizontalResizeType
        => (ResizeType)((byte)this.ResizeType & 0x0F);

    internal ResizeType VerticalResizeType
        => (ResizeType)((byte)this.ResizeType & 0xF0);

    internal ResizeModeAttribute(ResizeType resizeType)
    {
        this.ResizeType = resizeType;
    } // ctor (ResizeType)
} // internal sealed class ResizeModeAttribute : Attribute
