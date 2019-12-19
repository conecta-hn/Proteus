using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering.Printing;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.Proteus.Context;
using TheXDS.Proteus.Facturacion.Component;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Reporting;
using static TheXDS.Proteus.Proteus;

namespace TheXDS.Proteus.Api
{
    [Name("Datos de Conect@")]
    public class ConectaService : Service<ConectaContext>
    {
        public static CaiRango CurrentRango
        {
            get
            {
                return Service<ConectaService>().Context.CaiRangos
                    .OrderBy(p => FreeCorrelCount(p))
                    .FirstOrDefault(p => IsRangoOpen(p));
            }
        }

        public static bool IsRangoOpen(CaiRango rango)
        {
            return !(NextCorrel(rango) is null);
        }

        public static int? NextCorrel(CaiRango rango)
        {
            var l = Common.Sequence(rango.RangoInicial, rango.RangoFinal).ToList();
            foreach (var j in rango.Facturas) l.Remove(j.Correlativo);
            l.Sort();
            return l.FirstOrDefault();
        }

        public static int? NextCorrel()
        {
            return NextCorrel(CurrentRango);
        }

        public static int FreeCorrelCount(CaiRango rango)
        {
            var l = Common.Sequence(rango.RangoInicial, rango.RangoFinal).ToList();
            foreach (var j in rango.Facturas) l.Remove(j.Correlativo);
            return l.Count;
        }

        public static string GetFactNum(Factura f)
        {
            if (f is null) return null;
            CaiRango r;
            int? c;
            if (f.CaiRangoParent is null)
            {
                r = CurrentRango;
                c = NextCorrel(r);
            }
            else
            {
                r = f.CaiRangoParent;
                c = f.Correlativo;
            }
            if (r is null || c is null) return null;
            return $"{r.NumLocal:000}-{r.NumCaja:000}-{r.NumDocumento:00}-{c:00000000}";
        }

        public static bool IsProforma(Factura f)
        {
            return f.CaiRangoParent is null;
        }

        public static void AddFactura(Factura f, bool register, IFacturaInteractor i)
        {
            if (register)
            {
                RegisterFactura(f,i);
            }
        }

        public static void RegisterFactura(Factura f, IFacturaInteractor i)
        {
            f.CaiRangoParent = CurrentRango;
            f.Correlativo = NextCorrel(f.CaiRangoParent) ?? 1;
            PrintFactura(f, i);
        }

        public static void PrintFactura(Factura f, IFacturaInteractor i)
        {
            //_printer.Print(f);


            var doc = DocumentBuilder.CreateDocument();
            var section = AddFacturaHeader(doc, f);
            var table = AddItemsTable(section, i, out var cols);
            foreach (var j in f.Items)
            {
                AddItem(j, table, cols);
            }
            var lastCol = cols.Count - 1;
            AddSubTCol("Subtotal", f.SubTotal, table, lastCol);
            AddSubTCol("Subtotal gravable", f.SubTGravable, table, lastCol);
            AddSubTCol("Subtotal gravado", f.SubTGravado, table, lastCol);
            AddSubTCol("Subtotal final", f.SubTFinal, table, lastCol);
            AddSubTCol("Descuentos", f.Descuentos, table, lastCol);
            AddSubTCol("Cargos adicionales", f.OtrosCargos, table, lastCol);
            AddSubTCol("Total a pagar", f.SubTotal, table, lastCol).Format.Font.Bold = true;


            using var p = new MigraDocPrintDocument(doc)
            {
                PrinterSettings = new PrinterSettings()
            };
            p.Print();

                
            f.Impresa = true;
        }

        private static Row AddSubTCol(string label, decimal value, Table table, int lastCol)
        {
            var row = table.AddRow();
            row[lastCol - 2].MergeRight = 1;
            row[lastCol - 2].AddParagraph(label).Format.Alignment = ParagraphAlignment.Right;
            row[lastCol].AddParagraph(value.ToString("C"));
            return row;
        }

        private static void AddItem(ItemFactura j, Table table, IEnumerable<FacturaColumn> cols)
        {
            var row = table.AddRow();
            var c = 0;
            foreach (var k in cols)
            {
                row.Cells[c++].AddParagraph(k.Presenter(j) ?? string.Empty);
            }
        }
                
        private static Table AddItemsTable(Section section, IFacturaInteractor i, out List<FacturaColumn> cols)
        {
            var c = 0;
            cols = new[]
            {
                new FacturaColumn("#", _ => (++c).ToString()),
                new FacturaColumn("Item", f => f.Item.Name, 3.0),
            }.Concat(i?.ExtraColumns ?? new FacturaColumn[0]).Concat(new[]
            {
                new FacturaColumn("Cant.", f => f.Qty.ToString()),
                new FacturaColumn("Precio", f => f.StaticPrecio.ToString("C"), 2.0, true),
                new FacturaColumn("Descuentos", f => f.StaticDescuento.ToString("C"), 2.0, true),
                new FacturaColumn("Sub Total", f => f.SubTFinal.ToString("C"), 2.0, true),

            }).ToList();
            var colsTot = cols.Sum(p => p.RelaSize);
            var tblWidth = 17.0;

            var tbl = section.AddTable();
            foreach (var j in cols)
            {
                tbl.AddColumn(new Unit(j.RelaSize * tblWidth / colsTot, UnitType.Centimeter))
                    .Format.Alignment = j.Currency ? ParagraphAlignment.Right : ParagraphAlignment.Left;
            }
            var row = tbl.AddRow();
            foreach (var j in cols)
            {
                row.Cells[c++].AddParagraph(j.Header).Format.Font.Bold = true;                
            };
            c = 0; // la función lambda para la primera columna necesita este valor.
            return tbl;
        }

        private static Section AddFacturaHeader(Document doc, Factura f)
        {
            var retVal = doc.NewSection("FACTURA");
            //var table = retVal.Headers.FirstPage.AddTable();



            return retVal;
        }
    }
}
