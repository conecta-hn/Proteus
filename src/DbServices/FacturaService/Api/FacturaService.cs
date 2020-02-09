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
            if (f.Vuelto > 0m)
            {
                throw new InvalidOperationException("La factura tiene saldo pendiente.");
            }
            if (f.OtRef != null) f.OtRef.Facturado = true;
            f.CaiRangoParent = CurrentRango;
            f.Correlativo = NextCorrel(f.CaiRangoParent) ?? 1;
            PrintFactura(f, i);
        }

        public static async void PrintOt(OrdenTrabajo ot)
        {
            var ci = System.Globalization.CultureInfo.CreateSpecificCulture("es-HN");
            var p = PrintHeader("Ticket");
            p.BoldMode($"Código de orden: {ot.Id:000000}");
            p.Append($"Fecha de generacion: {ot.Timestamp:dd/MM/yyyy}");
            p.Append($"Fecha de entrega:    {ot.Entrega:dd/MM/yyyy}");
            p.Code128(ot.Id.ToString());
            p.Append($"Cliente: {ot.Cliente?.Name ?? "Consumidor final"}");
            p.Append($"RTN: {ot.Cliente?.Rtn ?? "9999-9999-999999"}");
            foreach (var j in ot.Cliente?.Phones.ToArray() ?? Array.Empty<Phone>())
            {
                p.Append($"Tel.   {j.Number}");
            }
            p.Append(new string('=', 40));
            p.Append("Cantidad     Precio      Subtotal");
            p.Append(new string('-', 40));
            var tot = 0m;
            foreach (var j in ot.Items)
            {
                p.AlignLeft();
                p.Append(j.Item.Name);
                var precio = j.Item.Precio + (j.Item.Precio * (decimal)((j.Item.Isv / 100f) ?? 0f));
                p.AlignRight();
                p.Append($"{j.Qty}        {precio.ToString("C", ci)}        {(j.Qty * precio).ToString("C", ci)}");
                tot += precio;
            }

            p.Append(new string('-', 40));

            void AddSubt(string label, decimal value)
            {
                p.Append($"{label:-25}: {value.ToString("C", ci)}");
            }
            p.AlignLeft();
            p.Append($"Total de prendas: {ot.Items.Sum(j => j.Qty)}");
            p.AlignRight();
            AddSubt($"Total a pagar", tot);

            p.AlignLeft();
            if (!ot.Notas.IsEmpty())
            {
                p.Append(new string('-', 40));
                p.Append(ot.Notas);
            }
            p.AlignCenter();
            p.Append("Gracias por preferirnos");
            p.BoldMode("ESTE TICKET NO ES UNA FACTURA");
            FooterAndPrint(p);

            await Task.Delay(3000);

            p = PrintHeader("Orden de trabajo");
            p.BoldMode($"Código de orden: {ot.Id:000000}");
            p.Append($"Fecha de generacion: {ot.Timestamp:dd/MM/yyyy}");
            p.Append($"Fecha de entrega:    {ot.Entrega:dd/MM/yyyy}");
            p.Code39(ot.Id.ToString());
            p.Append($"Cliente: {ot.Cliente?.Name ?? "Consumidor final"}");
            p.Append($"RTN: {ot.Cliente?.Rtn ?? "9999-9999-999999"}");
            foreach (var j in ot.Cliente?.Phones.ToArray() ?? Array.Empty<Phone>())
            {
                p.Append($"Tel.   {j.Number}");
            }
            p.Append(new string('=',40));
            p.Append("Descripcion              Cantidad");
            foreach (var j in ot.Items)
            {
                p.Append($"{j.Item.Name}: {j.Qty}");
            }
            p.Append(new string('-', 40));
            p.Append($"Total de prendas: {ot.Items.Sum(j => j.Qty)}");
            if (!ot.Notas.IsEmpty())
            {
                p.Append(new string('-', 40));
                p.Append(ot.Notas);
            }
            FooterAndPrint(p);
        }

        private static Printer GetPrinter()
        {
            return new Printer(GetEstation.Printer);
        }

        private static Printer PrintHeader(string title)
        {
            var e = GetEstation.Entidad;
            var p = GetPrinter();
            p.AlignCenter();
            p.Append(e.Name);
            if (!e.Banner.IsEmpty()) p.Append(e.Banner);
            p.Append(e.Address);
            p.Append($"{e.City}, {e.Country}");
            p.Append($"RTN: {e.Id}");
            p.Append(new string('-', 40));
            p.Append(title.ToUpper().Spell());
            p.Append(new string('-', 40));
            p.AlignLeft();
            return p;
        }

        private static void FooterAndPrint(Printer p)
        {
            //var nfo = new AssemblyInfo(typeof(Proteus).Assembly);
            //p.Append($"{nfo.Name} {nfo.InformationalVersion}");
            p.FullPaperCut();
            p.PrintDocument();
        }

        public static void PrintFactura(Factura f, IFacturaInteractor? i)
        {
            var ci = System.Globalization.CultureInfo.CreateSpecificCulture("es-HN");
            var p = PrintHeader("factura");
            void AddSubt(string label, decimal value)
            {
                p.Append($"{label:-25}: {value.ToString("C", ci)}");
            }
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
            p.Append($"{f.Cliente!.Exoneraciones.FirstOrDefault(p=>DateTime.Today.IsBetween(p.Timestamp, p.Void))}");
            p.Append("No. Registro SAG:");
            p.Append(new string('=', 40));
            p.Append("Descripcion");
            p.Append("Cantidad     Precio      Subtotal");
            p.Append(new string('-', 40));
            foreach (var j in f.Items)
            {
                p.Append(j.Item.Name);
                p.Append($"{j.Qty}    {j.StaticPrecio.ToString("C", ci)}    {j.SubTotal.ToString("C", ci)}");
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
            p.Append($"Atendido por: {GetCajero.UserEntity?.Name ?? GetCajero.UserId }");
            p.Append("Original - Cliente");
            p.Append("CC - Comercio");
            FooterAndPrint(p);
            f.Impresa = true;
        }
    }
}
