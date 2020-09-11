/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using BarcodeLib;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using TheXDS.MCART;

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
}
