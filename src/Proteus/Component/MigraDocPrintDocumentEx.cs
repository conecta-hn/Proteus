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


using System.Drawing.Printing;

namespace MigraDoc.Rendering.Printing
{
    /// <summary>
    /// MigraDocPrintDocumentEx does not use any MigraDoc classes in the interface.
    /// This allows consuming the class MigraDocPrintDocumentEx without referencing the GDI build of PDFsharp/MigraDoc.
    /// This allows assemblies that reference the WPF build or other builds of PDFsharp/MigraDoc to use this class for printing.
    /// To make this work, we have to pass the MigraDoc document as an MDDDL string.
    /// </summary>
    public class MigraDocPrintDocumentEx
    {
        public MigraDocPrintDocumentEx(string ddl)
        {
            _printDocument = new MigraDocPrintDocument(ddl);
        }

        public PrinterSettings PrinterSettings
        {
            get { return _printDocument.PrinterSettings; }
            set { _printDocument.PrinterSettings = value; }
        }

        public void Print()
        {
            _printDocument.Print();
        }

        private readonly MigraDocPrintDocument _printDocument;
    }

}
