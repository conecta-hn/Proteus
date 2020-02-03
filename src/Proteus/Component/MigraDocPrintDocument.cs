/*
Authors:
  Stefan Lange

Copyright (c) 2001-2009 empira Software GmbH, Cologne (Germany)

http://www.pdfsharp.com
http://www.migradoc.com
http://sourceforge.net/projects/pdfsharp

Modification for PDFsharp 1.50 beta 2
Authors:
  Thomas Hoevel

Copyright (c) 2015 Thomas Hoevel Software, Troisdorf, Germany.

Permission is hereby granted, free of charge, to any person obtaining a
copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/


using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.IO;
using PdfSharp;
using PdfSharp.Drawing;
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace MigraDoc.Rendering.Printing
{
    /// <summary>
    /// Represents a specialized System.Drawing.Printing.PrintDocument for MigraDoc documents.
    /// This component knows about MigraDoc and simplifies printing of MigraDoc documents.
    /// </summary>
    public class MigraDocPrintDocument : PrintDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigraDocPrintDocument"/> class. 
        /// </summary>
        public MigraDocPrintDocument()
        {
            DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            OriginAtMargins = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigraDocPrintDocument"/> class
        /// with the specified <see cref="DocumentRenderer"/> object.
        /// </summary>
        public MigraDocPrintDocument(DocumentRenderer renderer)
        {
            Renderer = renderer;
            DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            OriginAtMargins = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigraDocPrintDocument"/> class
        /// with the specified <see cref="Document"/> object.
        /// </summary>
        public MigraDocPrintDocument(Document document) : this()
        {
            Renderer = MakeRenderer(document);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigraDocPrintDocument"/> class
        /// with the specified <see cref="string"/> object.
        /// </summary>
        public MigraDocPrintDocument(string ddl) : this()
        {
            var document = DdlReader.DocumentFromString(ddl);
            Renderer = MakeRenderer(document);
        }

        private static DocumentRenderer MakeRenderer(Document document)
        {
            var renderer = new DocumentRenderer(document);
            renderer.PrepareDocument();
            return renderer;
        }

        /// <summary>
        /// Gets or sets the DocumentRenderer that prints the pages of the document.
        /// </summary>
        public DocumentRenderer? Renderer { get; set; }

        /// <summary>
        /// Gets or sets the page number that identifies the selected page. It it used on printing when 
        /// PrintRange.Selection is set.
        /// </summary>
        public int SelectedPage
        {
            get { return _selectedPage; }
            set { _selectedPage = value; }
        }
        int _selectedPage;

        /// <summary>
        /// Raises the <see cref="PrintDocument.BeginPrint"/> event. It is called after the <see cref="PrintDocument.Print"/> method is called and before the first page of the document prints.
        /// </summary>
        /// <param name="e">A <see cref="PrintEventArgs"/> that contains the event data.</param>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            Debug.Assert(Renderer != null, "Cannot print without a MigraDoc.Rendering.DocumentRenderer.");

            base.OnBeginPrint(e);
            if (!e.Cancel)
            {
                switch (PrinterSettings.PrintRange)
                {
                    case PrintRange.AllPages:
                        _pageNumber = 1;
                        _pageCount = Renderer.FormattedDocument.PageCount;
                        break;

                    case PrintRange.SomePages:
                        _pageNumber = PrinterSettings.FromPage;
                        _pageCount = PrinterSettings.ToPage - PrinterSettings.FromPage + 1;
                        break;

                    case PrintRange.Selection:
                        _pageNumber = _selectedPage;
                        _pageCount = 1;
                        break;

                    default:
                        Debug.Assert(false, "Invalid PrinterRange.");
                        e.Cancel = true;
                        break;
                }
            }
        }
        int _pageNumber = -1;
        int _pageCount;

        /// <summary>
        /// Raises the <see cref="PrintDocument.QueryPageSettings"/> event. It is called immediately before each <see cref="PrintDocument.PrintPage"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryPageSettingsEventArgs"/> that contains the event data.</param>
        protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            base.OnQueryPageSettings(e);
            if (!e.Cancel)
            {
                var settings = e.PageSettings;
                var pageInfo = Renderer?.FormattedDocument.GetPageInfo(_pageNumber);

                // set portrait/landscape
                settings.Landscape = pageInfo?.Orientation == PageOrientation.Landscape;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Drawing.Printing.PrintDocument.PrintPage"/> event. It is called before a page prints.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Printing.PrintPageEventArgs"/> that contains the event data.</param>
        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            if (!e.Cancel)
            {
                try
                {
                    var graphics = e.Graphics;
                    var hdc = graphics.GetHdc();
                    var xOffset = GetDeviceCaps(hdc, _physicalOffsetX);
                    var yOffset = GetDeviceCaps(hdc, _physicalOffsetY);
                    graphics.ReleaseHdc(hdc);
                    graphics.TranslateTransform(-xOffset * 100 / graphics.DpiX, -yOffset * 100 / graphics.DpiY);
                    // Recall: Width and Height are exchanged when settings.Landscape is true.
                    var size = new XSize(e.PageSettings.Bounds.Width / 100.0 * 72, e.PageSettings.Bounds.Height / 100.0 * 72);
                    const float scale = 100f / 72f;
                    graphics.ScaleTransform(scale, scale);
                    // draw line A4 portrait
                    //graphics.DrawLine(Pens.Red, 0, 0, 21f / 2.54f * 72, 29.7f / 2.54f * 72);
                    var gfx = XGraphics.FromGraphics(graphics, size);
                    Renderer?.RenderPage(gfx, _pageNumber);
                }
                catch
                {
                    e.Cancel = true;
                }
                _pageNumber++;
                _pageCount--;
                e.HasMorePages = _pageCount > 0;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Drawing.Printing.PrintDocument.EndPrint"/> event. It is called when the last page of the document has printed.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Printing.PrintEventArgs"/> that contains the event data.</param>
        protected override void OnEndPrint(PrintEventArgs e)
        {
            base.OnEndPrint(e);
            _pageNumber = -1;
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int capability);
        const int _physicalOffsetX = 112; // Physical Printable Area x margin
        const int _physicalOffsetY = 113; // Physical Printable Area y margin
    }
}
