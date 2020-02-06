/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using MigraDoc.DocumentObjectModel;
using TheXDS.MCART.Component;
using TheXDS.Proteus.Resources;
using C2 = MigraDoc.DocumentObjectModel.Color;

namespace TheXDS.Proteus.Reporting
{
    public static class DocumentBuilder
    {
        public static Document CreateDocument() => new Document();
        public static Section NewSection(this Document doc, string title)
        {
            var retVal = doc.AddSection();
            retVal.PageSetup.DifferentFirstPageHeaderFooter = true;
            retVal.PageSetup.TopMargin = new Unit(1.5, UnitType.Inch);
            retVal.PageSetup.PageFormat = PageFormat.Letter;
            AddMainHeader(retVal.Headers.Primary);
            AddMainHeader(retVal.Headers.FirstPage);
            AddTitle(retVal.Headers.FirstPage, title);
            AddFooter(retVal.Footers.Primary);
            AddFooter(retVal.Footers.FirstPage);
            return retVal;

        }
        public static void FormatAsTitle(this ParagraphFormat f, double fontSize = 16)
        {
            f.SpaceBefore = new Unit(1, UnitType.Centimeter);
            f.Alignment = ParagraphAlignment.Center;
            f.Font.Size = new Unit(fontSize);
            f.SpaceAfter = new Unit(0.5, UnitType.Centimeter);
        }
        public static void AddLine(this Borders p)
        {
            p.Bottom = new Border
            {
                Color = C2.FromRgb(0, 0, 0),
                Width = new Unit(1)
            };
        }
        public static void AddLine(this Paragraph p)
        {
            AddLine(p.Format.Borders);
        }

        private static void AddTitle(HeaderFooter header, string title)
        {
            var p = header.AddParagraph(title);
            FormatAsTitle(p.Format, 20);
            p.Format.Borders.Top = new Border
            {
                Color = C2.FromRgb(0, 0, 0),
                Width = new Unit(1)
            };
            AddLine(p);
        }
        private static void AddMainHeader(HeaderFooter header)
        {
            var p = header.AddParagraph();
            p.Format.Alignment = ParagraphAlignment.Center;
            p.Format.SpaceAfter = new Unit(0.5, UnitType.Centimeter);
            var img = p.AddImage($"base64:{Art.FullLogo}");
            img.Resolution = 400;
        }
        private static void AddFooter(HeaderFooter footer)
        {
            footer.AddParagraph("== Gracias por su compra ==");
            var i = new AssemblyInfo();
            footer.AddParagraph($"{i.Name} {i.InformationalVersion}");
        }
    }
}