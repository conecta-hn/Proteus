/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using BarcodeLib;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TheXDS.MCART;
using WpfScreenHelper;

namespace TheXDS.Proteus.FacturacionUi.Lib
{
    internal static class BarcodeHelper
    {
        public static Visual RenderBarcode(TYPE barcodeType, string id)
        {
            var v = new DrawingVisual();
            using var dc = v.RenderOpen();
            using var barcode = new Barcode
            {
                IncludeLabel = true,
                LabelPosition = LabelPositions.BOTTOMCENTER,
                ImageFormat = System.Drawing.Imaging.ImageFormat.Png,
                LabelFont = new Font("Consolas", 24)
            };
            var img = barcode.Encode(barcodeType, id);
            dc.DrawImage(img.ToSource(), new Rect { Width = img.Width, Height = img.Height });
            return v;
        }
    }

    internal static class ScreenHelper
    {
        public static void ToScreen(this Window window, byte screen)
        {
            var sc = Screen.AllScreens.ToList();
            var s = (screen >= sc.Count ? sc.Last() : sc[screen-1]).Bounds;
            window.Top = s.Top;
            window.Left = s.Left;
            window.Width = s.Width;
            window.Height = s.Height;
            window.WindowState = WindowState.Maximized;
        }
    }
}
