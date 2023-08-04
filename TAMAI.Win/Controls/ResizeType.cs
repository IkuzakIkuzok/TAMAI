
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win.Controls;

internal enum ResizeType : byte
{
    Fixed = 0x00,

    AdjustWidth = 0x01,
    AdjustLeft = 0x02,
    AdjustHeight = 0x10,
    AdjustTop = 0x20,

    AdjustSize = AdjustWidth | AdjustHeight,
    AdjustPosition = AdjustLeft | AdjustTop,
} // internal enum ResizeType : byte
