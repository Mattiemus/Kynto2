namespace Spark.UI
{
    using System.Collections.Generic;

    using Math;

    public static class Brushes
    {
        private static Dictionary<Color, SolidColorBrush> AllBrushes;

        public static SolidColorBrush AliceBlue => GetBrush(Color.AliceBlue);
        public static SolidColorBrush AntiqueWhite => GetBrush(Color.AntiqueWhite);
        public static SolidColorBrush Aqua => GetBrush(Color.Aqua);
        public static SolidColorBrush Aquamarine => GetBrush(Color.Aquamarine);
        public static SolidColorBrush Azure => GetBrush(Color.Azure);
        public static SolidColorBrush Beige => GetBrush(Color.Beige);
        public static SolidColorBrush Bisque => GetBrush(Color.Bisque);
        public static SolidColorBrush Black => GetBrush(Color.Black);
        public static SolidColorBrush BlanchedAlmond => GetBrush(Color.BlanchedAlmond);
        public static SolidColorBrush Blue => GetBrush(Color.Blue);
        public static SolidColorBrush BlueViolet => GetBrush(Color.BlueViolet);
        public static SolidColorBrush Brown => GetBrush(Color.Brown);
        public static SolidColorBrush BurlyWood => GetBrush(Color.BurlyWood);
        public static SolidColorBrush CadetBlue => GetBrush(Color.CadetBlue);
        public static SolidColorBrush Chartreuse => GetBrush(Color.Chartreuse);
        public static SolidColorBrush Chocolate => GetBrush(Color.Chocolate);
        public static SolidColorBrush Coral => GetBrush(Color.Coral);
        public static SolidColorBrush CornflowerBlue => GetBrush(Color.CornflowerBlue);
        public static SolidColorBrush Cornsilk => GetBrush(Color.Cornsilk);
        public static SolidColorBrush Crimson => GetBrush(Color.Crimson);
        public static SolidColorBrush Cyan => GetBrush(Color.Cyan);
        public static SolidColorBrush DarkBlue => GetBrush(Color.DarkBlue);
        public static SolidColorBrush DarkCyan => GetBrush(Color.DarkCyan);
        public static SolidColorBrush DarkGoldenrod => GetBrush(Color.DarkGoldenrod);
        public static SolidColorBrush DarkGray => GetBrush(Color.DarkGray);
        public static SolidColorBrush DarkGreen => GetBrush(Color.DarkGreen);
        public static SolidColorBrush DarkKhaki => GetBrush(Color.DarkKhaki);
        public static SolidColorBrush DarkMagenta => GetBrush(Color.DarkMagenta);
        public static SolidColorBrush DarkOliveGreen => GetBrush(Color.DarkOliveGreen);
        public static SolidColorBrush DarkOrange => GetBrush(Color.DarkOrange);
        public static SolidColorBrush DarkOrchid => GetBrush(Color.DarkOrchid);
        public static SolidColorBrush DarkRed => GetBrush(Color.DarkRed);
        public static SolidColorBrush DarkSalmon => GetBrush(Color.DarkSalmon);
        public static SolidColorBrush DarkSeaGreen => GetBrush(Color.DarkSeaGreen);
        public static SolidColorBrush DarkSlateBlue => GetBrush(Color.DarkSlateBlue);
        public static SolidColorBrush DarkSlateGray => GetBrush(Color.DarkSlateGray);
        public static SolidColorBrush DarkTurquoise => GetBrush(Color.DarkTurquoise);
        public static SolidColorBrush DarkViolet => GetBrush(Color.DarkViolet);
        public static SolidColorBrush DeepPink => GetBrush(Color.DeepPink);
        public static SolidColorBrush DeepSkyBlue => GetBrush(Color.DeepSkyBlue);
        public static SolidColorBrush DimGray => GetBrush(Color.DimGray);
        public static SolidColorBrush DodgerBlue => GetBrush(Color.DodgerBlue);
        public static SolidColorBrush Firebrick => GetBrush(Color.Firebrick);
        public static SolidColorBrush FloralWhite => GetBrush(Color.FloralWhite);
        public static SolidColorBrush ForestGreen => GetBrush(Color.ForestGreen);
        public static SolidColorBrush Fuchsia => GetBrush(Color.Fuchsia);
        public static SolidColorBrush Gainsboro => GetBrush(Color.Gainsboro);
        public static SolidColorBrush GhostWhite => GetBrush(Color.GhostWhite);
        public static SolidColorBrush Gold => GetBrush(Color.Gold);
        public static SolidColorBrush Goldenrod => GetBrush(Color.Goldenrod);
        public static SolidColorBrush Gray => GetBrush(Color.Gray);
        public static SolidColorBrush Green => GetBrush(Color.Green);
        public static SolidColorBrush GreenYellow => GetBrush(Color.GreenYellow);
        public static SolidColorBrush Honeydew => GetBrush(Color.Honeydew);
        public static SolidColorBrush HotPink => GetBrush(Color.HotPink);
        public static SolidColorBrush IndianRed => GetBrush(Color.IndianRed);
        public static SolidColorBrush Indigo => GetBrush(Color.Indigo);
        public static SolidColorBrush Ivory => GetBrush(Color.Ivory);
        public static SolidColorBrush Khaki => GetBrush(Color.Khaki);
        public static SolidColorBrush Lavender => GetBrush(Color.Lavender);
        public static SolidColorBrush LavenderBlush => GetBrush(Color.LavenderBlush);
        public static SolidColorBrush LawnGreen => GetBrush(Color.LawnGreen);
        public static SolidColorBrush LemonChiffon => GetBrush(Color.LemonChiffon);
        public static SolidColorBrush LightBlue => GetBrush(Color.LightBlue);
        public static SolidColorBrush LightCoral => GetBrush(Color.LightCoral);
        public static SolidColorBrush LightCyan => GetBrush(Color.LightCyan);
        public static SolidColorBrush LightGoldenrodYellow => GetBrush(Color.LightGoldenrodYellow);
        public static SolidColorBrush LightGray => GetBrush(Color.LightGray);
        public static SolidColorBrush LightGreen => GetBrush(Color.LightGreen);
        public static SolidColorBrush LightPink => GetBrush(Color.LightPink);
        public static SolidColorBrush LightSalmon => GetBrush(Color.LightSalmon);
        public static SolidColorBrush LightSeaGreen => GetBrush(Color.LightSeaGreen);
        public static SolidColorBrush LightSkyBlue => GetBrush(Color.LightSkyBlue);
        public static SolidColorBrush LightSlateGray => GetBrush(Color.LightSlateGray);
        public static SolidColorBrush LightSteelBlue => GetBrush(Color.LightSteelBlue);
        public static SolidColorBrush LightYellow => GetBrush(Color.LightYellow);
        public static SolidColorBrush Lime => GetBrush(Color.Lime);
        public static SolidColorBrush LimeGreen => GetBrush(Color.LimeGreen);
        public static SolidColorBrush Linen => GetBrush(Color.Linen);
        public static SolidColorBrush Magenta => GetBrush(Color.Magenta);
        public static SolidColorBrush Maroon => GetBrush(Color.Maroon);
        public static SolidColorBrush MediumAquamarine => GetBrush(Color.MediumAquamarine);
        public static SolidColorBrush MediumBlue => GetBrush(Color.MediumBlue);
        public static SolidColorBrush MediumOrchid => GetBrush(Color.MediumOrchid);
        public static SolidColorBrush MediumPurple => GetBrush(Color.MediumPurple);
        public static SolidColorBrush MediumSeaGreen => GetBrush(Color.MediumSeaGreen);
        public static SolidColorBrush MediumSlateBlue => GetBrush(Color.MediumSlateBlue);
        public static SolidColorBrush MediumSpringGreen => GetBrush(Color.MediumSpringGreen);
        public static SolidColorBrush MediumTurquoise => GetBrush(Color.MediumTurquoise);
        public static SolidColorBrush MediumVioletRed => GetBrush(Color.MediumVioletRed);
        public static SolidColorBrush MidnightBlue => GetBrush(Color.MidnightBlue);
        public static SolidColorBrush MintCream => GetBrush(Color.MintCream);
        public static SolidColorBrush MistyRose => GetBrush(Color.MistyRose);
        public static SolidColorBrush Moccasin => GetBrush(Color.Moccasin);
        public static SolidColorBrush NavajoWhite => GetBrush(Color.NavajoWhite);
        public static SolidColorBrush Navy => GetBrush(Color.Navy);
        public static SolidColorBrush OldLace => GetBrush(Color.OldLace);
        public static SolidColorBrush Olive => GetBrush(Color.Olive);
        public static SolidColorBrush OliveDrab => GetBrush(Color.OliveDrab);
        public static SolidColorBrush Orange => GetBrush(Color.Orange);
        public static SolidColorBrush OrangeRed => GetBrush(Color.OrangeRed);
        public static SolidColorBrush Orchid => GetBrush(Color.Orchid);
        public static SolidColorBrush PaleGoldenrod => GetBrush(Color.PaleGoldenrod);
        public static SolidColorBrush PaleGreen => GetBrush(Color.PaleGreen);
        public static SolidColorBrush PaleTurquoise => GetBrush(Color.PaleTurquoise);
        public static SolidColorBrush PaleVioletRed => GetBrush(Color.PaleVioletRed);
        public static SolidColorBrush PapayaWhip => GetBrush(Color.PapayaWhip);
        public static SolidColorBrush PeachPuff => GetBrush(Color.PeachPuff);
        public static SolidColorBrush Peru => GetBrush(Color.Peru);
        public static SolidColorBrush Pink => GetBrush(Color.Pink);
        public static SolidColorBrush Plum => GetBrush(Color.Plum);
        public static SolidColorBrush PowderBlue => GetBrush(Color.PowderBlue);
        public static SolidColorBrush Purple => GetBrush(Color.Purple);
        public static SolidColorBrush Red => GetBrush(Color.Red);
        public static SolidColorBrush RosyBrown => GetBrush(Color.RosyBrown);
        public static SolidColorBrush RoyalBlue => GetBrush(Color.RoyalBlue);
        public static SolidColorBrush SaddleBrown => GetBrush(Color.SaddleBrown);
        public static SolidColorBrush Salmon => GetBrush(Color.Salmon);
        public static SolidColorBrush SandyBrown => GetBrush(Color.SandyBrown);
        public static SolidColorBrush SeaGreen => GetBrush(Color.SeaGreen);
        public static SolidColorBrush SeaShell => GetBrush(Color.SeaShell);
        public static SolidColorBrush Sienna => GetBrush(Color.Sienna);
        public static SolidColorBrush Silver => GetBrush(Color.Silver);
        public static SolidColorBrush SkyBlue => GetBrush(Color.SkyBlue);
        public static SolidColorBrush SlateBlue => GetBrush(Color.SlateBlue);
        public static SolidColorBrush SlateGray => GetBrush(Color.SlateGray);
        public static SolidColorBrush Snow => GetBrush(Color.Snow);
        public static SolidColorBrush SpringGreen => GetBrush(Color.SpringGreen);
        public static SolidColorBrush SteelBlue => GetBrush(Color.SteelBlue);
        public static SolidColorBrush Tan => GetBrush(Color.Tan);
        public static SolidColorBrush Teal => GetBrush(Color.Teal);
        public static SolidColorBrush Thistle => GetBrush(Color.Thistle);
        public static SolidColorBrush Tomato => GetBrush(Color.Tomato);
        public static SolidColorBrush TransparentBlack => GetBrush(Color.TransparentBlack);
        public static SolidColorBrush Turquoise => GetBrush(Color.Turquoise);
        public static SolidColorBrush Violet => GetBrush(Color.Violet);
        public static SolidColorBrush Wheat => GetBrush(Color.Wheat);
        public static SolidColorBrush White => GetBrush(Color.White);
        public static SolidColorBrush WhiteSmoke => GetBrush(Color.WhiteSmoke);
        public static SolidColorBrush Yellow => GetBrush(Color.Yellow);
        public static SolidColorBrush YellowGreen => GetBrush(Color.YellowGreen);

        internal static SolidColorBrush GetBrush(Color color)
        {
            SolidColorBrush result = default(SolidColorBrush);

            if (AllBrushes == null)
            {
                AllBrushes = new Dictionary<Color, SolidColorBrush>();
            }
            else
            {
                if (AllBrushes.ContainsKey(color))
                {
                    result = AllBrushes[color];
                }
            }

            if (result == null)
            {
                result = new SolidColorBrush(color);
                AllBrushes[color] = result;
            }

            return result;
        }
    }
}
