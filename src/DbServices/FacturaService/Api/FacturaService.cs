using System;
using System.Collections.Generic;
using System.Text;
using TheXDS.Proteus.Context;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Security.Password;
using TheXDS.MCART.Types.Extensions;
using static TheXDS.Proteus.Proteus;
using TheXDS.Proteus.Reporting;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using TheXDS.Proteus.Component;
using MigraDoc.Rendering.Printing;
using System.Drawing.Printing;
using ESC_POS_USB_NET.Printer;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Api
{
    public class FacturaService : Service<FacturaContext>
    {
        public static List<PaymentSource> PaymentSources { get; } = Objects.FindAllObjects<PaymentSource>().ToList();

        public static Estacion GetEstation => GetStation<Estacion>();
        public static Cajero GetCajero => GetUser<Cajero>();
        public static CajaOp GetCajaOp
        {
            get
            {
                var c = GetCajero;
                var s = GetEstation;
                if (new object[] { c, s }.IsAnyNull()) return null;
                return Service<FacturaService>().FirstOrDefault<CajaOp>(p => p.CloseTimestamp == null && p.Cajero.Id == c.Id && p.Estacion.Id == s.Id);
            }
        }
        public static string CajeroName => Proteus.Session?.Name ?? "Estación de facturación";
        public static CaiRango CurrentRango
        {
            get
            {
                return GetEstation.RangosAsignados
                    .OrderBy(p => FreeCorrelCount(p))
                    .FirstOrDefault(p => IsRangoOpen(p));
            }
        }
        public static bool IsThisStation => !(GetEstation is null);
        public static bool IsThisCajero => !(GetCajero is null);
        public static bool IsCajaOpOpen => !(GetCajaOp is null);
        public static IQueryable<CaiRango> UnassignedRangos => Service<FacturaService>()!.All<CaiRango>().Where(p => p.AssignedTo == null);

        public static bool IsRangoOpen(CaiRango rango)
        {
            return !(NextCorrel(rango) is null);
        }
        public static int? NextCorrel(CaiRango rango)
        {
            var l =rango.GetFreeCorrels();
            l.Sort();
            return l.FirstOrDefault();
        }
        public static int? NextCorrel() => NextCorrel(CurrentRango);
        public static int FreeCorrelCount(CaiRango rango) => rango.GetFreeCorrels().Count;
        public static int FreeCorrelCount() => CurrentRango.GetFreeCorrels().Count;
        public static string? GetFactNum(Factura f)
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
        public static void AddFactura(Factura f, bool register, IFacturaInteractor? i)
        {
            if (register)
            {
                RegisterFactura(f, i);
            }
            GetCajaOp.Facturas.Add(f);
        }
        public static void RegisterFactura(Factura f, IFacturaInteractor? i)
        {
            f.CaiRangoParent = CurrentRango;
            f.Correlativo = NextCorrel(f.CaiRangoParent) ?? 1;
            PrintFactura(f, i);
        }

        public static void PrintFactura(Factura f, IFacturaInteractor? i)
        {
            var ci = System.Globalization.CultureInfo.CreateSpecificCulture("es-HN");
            var p = new Printer("Generic / Text Only");
            var e = GetEstation.Entidad;
            void AddSubt(string label, decimal value)
            {
                p.AlignLeft();
                p.AppendWithoutLf($"{label}:");
                p.AlignRight();
                p.Append(value.ToString("C", ci));
            }
            p.AlignCenter();
            p.Append(e.Name);
            if (!e.Banner.IsEmpty()) p.Append(e.Banner);
            p.Append(e.Address);
            p.Append($"{e.City}, {e.Country}");
            p.Separator();
            p.Append("F A C T U R A");
            p.Separator();
            p.AlignLeft();
            p.Append($"RTN: {e.Id}");
            p.Append($"C.A.I.:{f.CaiRangoParent.Parent.Id}");
            p.Append($"Rango autoriz. de facturación: {f.CaiRangoParent.RangoString()}");
            p.Append($"Fecha límite de emisión: {f.CaiRangoParent.Parent.Void:dd/MMM/yyyy}");
            p.Append($"Factura # {f.FactNum}");
            p.Append($"Cliente: {f.Cliente.Name ?? "Consumidor final"}");
            p.Append($"RTN del cliente: {f.Cliente.Rtn}");
            p.Append("No. Compra exenta:");
            p.Append($"No. constancia registro exonerado: {f.Cliente!.Exoneraciones.FirstOrDefault(p=>DateTime.Today.IsBetween(p.Timestamp, p.Void))}");
            p.Append("No. Registro SAG:");
            p.Separator('=');
            p.Append("Descripción");
            p.Append("Cantidad        Precio        Subtotal");
            p.Separator();
            foreach (var j in f.Items)
            {
                p.Append(j.Item.Name);
                p.AlignLeft();
                p.AppendWithoutLf(j.Qty.ToString());
                p.AlignCenter();
                p.AppendWithoutLf(j.StaticPrecio.ToString("C", ci));
                p.AlignRight();
                p.Append(j.SubTotal.ToString("C", ci));
            }
            AddSubt("Subtotal", f.SubTotal);
            AddSubt("15% ISV", f.SubTGravable);
            AddSubt("Gravado 15%", f.SubTGravado);
            AddSubt("Descuentos", f.Descuentos);
            AddSubt("TOTAL", f.Total);
            foreach (var j in f.Payments)
            {
                AddSubt(j.ResolveSource()?.Name ?? "Pago misc.", j.Amount);
            }
            AddSubt("Cambio", f.Vuelto);
            p.Separator('=');
            p.AlignLeft();
            p.Append("*Gracias por su compra.*");
            var nfo = new AssemblyInfo(typeof(Proteus).Assembly);
            p.Append($"{nfo.Name} {nfo.InformationalVersion}");
            p.FullPaperCut();
            p.PrintDocument();

            //var doc = DocumentBuilder.CreateDocument();
            //var section = AddFacturaHeader(doc, f);
            //AddItemsTable(section, i, f);

            //using var p = new MigraDocPrintDocument(doc)
            //{
            //    PrinterSettings = new PrinterSettings()
            //};
            //p.Print();
            f.Impresa = true;
        }
        
        private static Section AddFacturaHeader(Document doc, Factura f)
        {
            var retVal = doc.NewSection("FACTURA");            
            return retVal;
        }

        private static Table AddItemsTable(Section section, IFacturaInteractor i, Factura f)
        {
            var ci = System.Globalization.CultureInfo.CreateSpecificCulture("es-HN");
            var ic = 0;
            var c = 0;
            var cols = new[]
            {
                new FacturaColumn("#", _ => (++ic).ToString()),
                new FacturaColumn("Item", f => f.Item.Name, 4.0),
            }.Concat(i?.ExtraColumns ?? Array.Empty<FacturaColumn>()).Concat(new[]
            {
                new FacturaColumn("Cant.", f => f.Qty.ToString(), 0.5),
                new FacturaColumn("Precio", f => f.StaticPrecio.ToString("C", ci), 2.0, true),
                new FacturaColumn("Descuentos", f => f.StaticDescuento.ToString("C", ci), 2.0, true),
                new FacturaColumn("Sub Total", f => f.SubTFinal.ToString("C", ci), 2.0, true),
            }).ToList();
            var colsTot = cols.Sum(p => p.RelaSize);
            var tblWidth = 18.5;
            var tbl = section.AddTable();
            foreach (var j in cols)
            {
                tbl.AddColumn(new Unit(j.RelaSize * tblWidth / colsTot, UnitType.Centimeter))
                    .Format.Alignment = j.Currency ? ParagraphAlignment.Right : ParagraphAlignment.Left;
            }

            void AddSubt(string label, decimal value)
            {
                var r = tbl.AddRow();
                r[cols.Count - 2].AddParagraph($"{label}:");
                r[cols.Count - 1].AddParagraph(value.ToString("C", ci));

            }

            var row = tbl.AddRow();
            foreach (var j in cols)
            {
                row.Cells[c++].AddParagraph(j.Header).Format.Font.Bold = true;
            };
            row.Borders = new Borders() { Bottom = new Border() { Color = Colors.Black, Width = 1.0f } };

            foreach (var j in f.Items)
            {
                row = tbl.AddRow();
                c = 0;
                foreach (var k in cols)
                {
                    row[c++].AddParagraph(k.Presenter(j));
                }
            }
            row = tbl.AddRow();
            row.Borders = new Borders() { Bottom = new Border() { Color = Colors.Black, Width = 1.0f } };
            row[1].AddParagraph("-- Última línea --").Format.Alignment = ParagraphAlignment.Center;
            
            AddSubt("Subtotal", f.SubTotal);
            AddSubt("15% ISV", f.SubTGravable);
            AddSubt("Gravado 15%", f.SubTGravado);
            //AddSubt("Subtotal final", f.SubTFinal);
            AddSubt("Descuentos", f.Descuentos);
            AddSubt("TOTAL", f.Total);
            foreach (var j in f.Payments)
            {
                AddSubt(j.ResolveSource().Name, j.Amount);
            }
            AddSubt("Cambio", -f.Vuelto);

            return tbl;
        }        
    }
}
