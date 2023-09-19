
// (c) 2023 Kazuki KOHZUKI

using WMBrush = System.Windows.Media.Brush;
using WMColor = System.Windows.Media.Color;

namespace TAMAI.Win.Controls;

internal class ColorGradient
{
    protected Color startColor, endColor;
    protected bool gammaCorrection = true;
    protected float gamma = 2.2f;

    protected readonly Color[] colors;

    internal ColorWrapper this[int position] => new(this.colors[position]);

    internal ColorWrapper this[float position] => this[(int)(position * this.colors.Length)];

    internal int Width => this.colors.Length;

    internal Color StartColor
    {
        get => this.startColor;
        set
        {
            if (this.startColor == value) return;
            this.startColor = value;
            SetColors();
        }
    }

    internal Color EndColor
    {
        get => this.endColor;
        set
        {
            if (this.endColor == value) return;
            this.endColor = value;
            SetColors();
        }
    }

    internal bool GammaCorrection
    {
        get => this.gammaCorrection;
        set
        {
            if (this.gammaCorrection == value) return;
            this.gammaCorrection = value;
            SetColors();
        }
    }

    internal float Gamma
    {
        get => this.gamma;
        set
        {
            if (this.gamma == value) return;
            this.gamma = value;
            SetColors();
        }
    }

    internal ColorGradient(Color startColor, Color endColor) : this(startColor, endColor, 100) { }

    internal ColorGradient(Color startColor, Color endColor, int width)
    {
        this.colors = new Color[width];
        this.startColor = startColor;
        this.endColor = endColor;
        SetColors();
    } // internal ColorGradient(Color, Color, int, int)

    protected virtual void SetColors()
    {
        this.colors[0] = this.startColor;
        this.colors[^1] = this.endColor;
        for (var i = 1; i < this.Width - 1; i++)
        {
            var r = (int)(this.startColor.R + (this.endColor.R - this.startColor.R) * i / (this.Width - 1f));
            var g = (int)(this.startColor.G + (this.endColor.G - this.startColor.G) * i / (this.Width - 1f));
            var b = (int)(this.startColor.B + (this.endColor.B - this.startColor.B) * i / (this.Width - 1f));
            if (this.gammaCorrection)
            {
                var gamma = 1 / this.gamma;
                r = (int)(Math.Pow(r / 255f, gamma) * 255);
                g = (int)(Math.Pow(g / 255f, gamma) * 255);
                b = (int)(Math.Pow(b / 255f, gamma) * 255);
            }
            this.colors[i] = Color.FromArgb(r, g, b);
        }
    } // protected virtual void SetColors ()

    internal class ColorWrapper
    {
        private readonly Color color;

        internal ColorWrapper(Color color)
        {
            this.color = color;
        } // ctor (Color)   

        public static implicit operator Color(ColorWrapper wrapper) => wrapper.color;

        public static implicit operator WMBrush(ColorWrapper wrapper) => new System.Windows.Media.SolidColorBrush(WMColor.FromArgb(wrapper.color.A, wrapper.color.R, wrapper.color.G, wrapper.color.B));
    } // internal class ColorWrapper
} // internal class ColorGradient
