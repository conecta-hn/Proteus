using ESC_POS_USB_NET.Printer;
using System;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;

namespace TheXDS.Proteus.PosFacturaPrinter
{
    [Name("Impresora de POS"), Description("Utiliza un sistema de impresión compatible con POS para imprimir la factura.")]
    [Guid("8e4ebc2e-da07-4ecb-abcd-fd0ecc7d7ea1")]
    public class PosFacturaPrinter : FacturaPrintDriver
    {
        private static Printer GetPrinter()
        {
            return new Printer(FacturaService.GetEstation?.Printer 
                ?? PrinterSettings.InstalledPrinters[0] 
                ?? throw new Exception("No hay ninguna impresora disponible."));
        }

        private static Printer PrintHeader(string title)
        {
            var e = FacturaService.GetEstation?.Entidad;
            var p = GetPrinter();
            p.AlignCenter();
            p.Append(e?.Name);
            if (!e?.Banner.IsEmpty() ?? false) p.Append(e!.Banner);
            p.Append(e?.Address);
            p.Append($"{e?.City}, {e?.Country}");
            p.Append($"RTN: {e?.Id}");
            p.Append(new string('-', 40));
            p.Append(title.ToUpper().Spell());
            p.Append(new string('-', 40));
            p.AlignLeft();
            return p;
        }

        private static void FooterAndPrint(Printer p)
        {
            p.FullPaperCut();
            p.PrintDocument();
        }

        public override void PrintFactura(Factura f, IFacturaInteractor? i)
        {
            var ci = System.Globalization.CultureInfo.CreateSpecificCulture("es-HN");
            var p = PrintHeader("factura");
            void AddSubt(string label, decimal value) => p.Append($"{$"{label}:",25}{value.ToString("C", ci),15}");
            p.Append("C.A.I.:");
            p.Append($"{f.CaiRangoParent.Parent.Id}");
            p.Append($"Rango autorizado de facturacion:");
            p.Append($"{f.CaiRangoParent.RangoString()}");
            p.Append($"Fecha lim. de emision: {f.CaiRangoParent.Parent.Void:dd/MM/yyyy}");
            p.Append(new string('-', 40));
            p.Append($"Factura # {f.FactNum}");
            p.Append($"Fecha de facturacion: {f.Timestamp:dd/MM/yyyy}");
            p.Append($"Cliente: {f.Cliente.Name ?? "Consumidor final"}");
            p.Append($"RTN: {f.Cliente?.Rtn ?? "9999-9999-999999"}");
            p.Append("No. Compra exenta:");
            p.Append("No. constancia registro exonerado:");
            p.Append($"{f.Cliente!.Exoneraciones.FirstOrDefault(p => DateTime.Today.IsBetween(p.Timestamp.Date, p.Void.Date + TimeSpan.FromDays(1)))?.Id}");
            p.Append("No. Registro SAG:");
            p.Append(new string('=', 40));
            p.Append("Descripcion");
            p.Append($"{"Cant.",-5}{"Precio",15}{"Subtotal", 20:C}");
            p.Append(new string('-', 40));
            foreach (var j in f.Items)
            {
                p.Append(j.Item.Name);
                p.Append($"{j.Qty,5}{j.StaticPrecio.ToString("C", ci),15}{j.SubTotal.ToString("C", ci),20}");
            }
            p.Append(new string('-', 40));
            p.AlignRight();
            AddSubt("Subtotal", f.SubTotal);
            AddSubt("15% ISV", f.SubTGravable);
            AddSubt("Gravado 15%", f.SubTGravado);
            AddSubt("Descuentos", f.Descuentos);
            AddSubt("TOTAL", f.Total);
            foreach (var j in f.Payments)
            {
                AddSubt(j.ResolveSource()?.Name ?? "Pago misc.", j.Amount);
            }
            AddSubt("Cambio", -f.Vuelto);
            if (!f.Notas.IsEmpty())
            {
                p.Append(new string('-', 40));
                p.Append(f.Notas);
            }
            p.Append(new string('=', 40));
            p.AlignLeft();
            p.Append("Gracias por su compra.");
            p.Append($"Atendido por: {FacturaService.GetCajero?.UserEntity?.Name ?? FacturaService.GetCajero?.UserId ?? TheXDS.Proteus.Proteus.Session?.Id}");
            p.Append("Original - Cliente");
            p.Append("CC - Comercio");
            FooterAndPrint(p);
            f.Impresa = true;
        }

        public override void PrintProforma(Factura f, IFacturaInteractor? i)
        {
            var ci = System.Globalization.CultureInfo.CreateSpecificCulture("es-HN");
            var p = PrintHeader("PROFORMA");
            p.BoldMode($"Código de orden: {f.Id:000000}");
            p.Append($"Fecha de generacion: {f.Timestamp:dd/MM/yyyy}");
            p.Code128(f.Id.ToString());
            p.Append($"Cliente: {f.Cliente?.Name ?? "Consumidor final"}");
            p.Append($"RTN: {f.Cliente?.Rtn ?? "9999-9999-999999"}");
            var exonerar = f.Cliente?.Exoneraciones.Any(p => DateTime.Today.IsBetween(p.Timestamp.Date, p.Void.Date + TimeSpan.FromDays(1))) ?? false;
            if (exonerar)
            {
                p.Append("No. constancia registro exonerado:");
                p.Append($"{f.Cliente!.Exoneraciones.FirstOrDefault(p => DateTime.Today.IsBetween(p.Timestamp, p.Void))?.Id}");
            }

            foreach (var j in f.Cliente?.Phones.ToArray() ?? Array.Empty<Phone>())
            {
                p.Append($"Tel.   {j.Number}");
            }
            p.Append(new string('=', 40));
            p.Append($"{"Cant.",-5}{"Precio",15}{"Subtotal",20:C}");
            p.Append(new string('-', 40));
            var tot = 0m;

            foreach (var j in f.Items)
            {
                p.AlignLeft();
                p.Append(j.Item.Name);
                var precio = j.Item.Precio;
                if (!exonerar)
                    precio += (j.Item.Precio * (decimal)((j.Item.Isv / 100f) ?? 0f));
                p.AlignRight();
                p.Append($"{j.Qty,-5}{precio.ToString("C", ci),15}{(j.Qty * precio).ToString("C", ci),20}");
                tot += precio * j.Qty;
            }

            p.Append(new string('-', 40));

            void AddSubt(string label, decimal value) => p.Append($"{$"{label}:",25}{value.ToString("C", ci),15}");
            p.AlignLeft();
            p.Append($"Total de prendas: {f.Items.Sum(j => j.Qty)}");
            p.AlignRight();
            AddSubt("Descuentos", f.Descuentos);
            AddSubt("Otros cargos", f.OtrosCargos);
            AddSubt($"Total a pagar", tot + f.OtrosCargos - f.Descuentos);

            p.AlignLeft();
            if (!f.Notas.IsEmpty())
            {
                p.Append(new string('-', 40));
                p.Append(f.Notas);
            }
            p.AlignCenter();
            p.Append("Gracias por preferirnos");
            p.BoldMode("ESTA PROFORMA NO ES UNA FACTURA");
            FooterAndPrint(p);
        }
    }
}
