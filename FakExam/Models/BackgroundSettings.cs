using System;

namespace FakExam.Models
{
    public enum BackgroundCategory
    {
        Material,
        Color,
        Image
    }

    public enum MaterialType
    {
        Mica,
        Acrylic
    }

    public enum MicaSubKind
    {
        Base,
        BaseAlt
    }

    public sealed class ImageMaskSettings
    {
        public bool Enabled { get; set; } = false;
        public string MaskColorHex { get; set; } = "#000000";
        public double MaskOpacity { get; set; } = 0.3; // 0~1
    }

    public sealed class BackgroundSettings
    {
        public BackgroundCategory Category { get; set; } = BackgroundCategory.Material;
        public MaterialType Material { get; set; } = MaterialType.Mica;
        public MicaSubKind MicaKind { get; set; } = MicaSubKind.Base;

        public string ColorHex { get; set; } = "#202020";

        public string? ImagePath { get; set; }
        public ImageMaskSettings Mask { get; set; } = new();
    }
}
