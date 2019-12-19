/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Misc;
using System.Drawing;
using System.Drawing.Printing;

namespace TheXDS.Proteus.Crud
{
    internal class BarcodeHelper
    {
        public static void MkBarcode(ModelBase obj)
        {
            var img = Internal.MakeBarcode(obj);

            var pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 100, 77);

            pd.DefaultPageSettings.Landscape = false;
            pd.PrintPage += (_, g) =>
            {
                int printHeight = 450;
                int printWidth = 400;
                int leftMargin = 20;
                int rightMargin = 0;

                img.RotateFlip(RotateFlipType.Rotate90FlipNone);

                g.Graphics.DrawImage(img, new Rectangle(leftMargin, rightMargin, printWidth, printHeight));
            };
            pd.Print();
        }

    }
}