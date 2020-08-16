using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheXDS.MCART;
using TheXDS.MCART.Exceptions;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Context;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;
using static TheXDS.Proteus.Proteus;

namespace TheXDS.Proteus.Api
{
    public class FacturaService : Service<FacturaContext>
    {
        public static List<PaymentSource> PaymentSources { get; } = Objects.FindAllObjects<PaymentSource>().ToList();
        public static List<FacturaPrintDriver> FactPrintDrivers { get; } = Objects.FindAllObjects<FacturaPrintDriver>().ToList();

        /// <summary>
        /// Obtiene una referencia al cliente genérico.
        /// </summary>
        public static Cliente GenericCliente => Service<FacturaService>()!.All<Cliente>().First();
        public static Estacion? GetEstation => GetStation<Estacion>();
        public static Cajero? GetCajero => GetUser<Cajero>();
        public static CajaOp? GetCajaOp
        {
            get
            {
                var c = GetCajero;
                var s = GetEstation;
                if (new object?[] { c, s }.IsAnyNull()) return null;
                return (Service<FacturaService>()?? throw new TamperException()).FirstOrDefault<CajaOp>(p => p.CloseTimestamp == null && p.Cajero.Id == c!.Id && p.Estacion.Id == s!.Id);
            }
        }
        public static string CajeroName => Proteus.Session?.Name ?? "Estación de facturación";
        public static CaiRango? CurrentRango
        {
            get
            {
                return GetEstation?.RangosAsignados
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
        public static string? GetFactNum(Factura? f)
        {
            if (f is null) return null;
            CaiRango? r;
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

        public static bool AddFactura(Factura f, bool register, IFacturaInteractor? i)
        {
            if (!IsCajaOpOpen)
            {
                MessageTarget?.Stop("No se puede registrar esta factura. La caja está cerrada.");
                return false;
            }
            if (register)
            {
                RegisterFactura(f, i);
            }
            GetCajaOp.Facturas.Add(f);
            return true;
        }

        public static bool RegisterFactura(Factura f, IFacturaInteractor? i)
        {
            if (f.Vuelto > 0m)
            {
                MessageTarget?.Stop("La factura tiene saldo pendiente.");
                return false;
            }
            f.CaiRangoParent = CurrentRango;
            f.Correlativo = NextCorrel(f.CaiRangoParent) ?? 1;
            try
            {
                PrintFactura(f, i);
            }
            catch (FileNotFoundException fnfex)
            {
                MessageTarget?.Warning($"Hubo un problema al cargar un componente necesario para imprimir la factura. Asegúrese que la aplicación se encuentre correctamente instalada.\n{fnfex.Message}");
            }
            catch
            {
                MessageTarget?.Warning("La factura se guardó, pero hubo un problema al imprimirla.");
            }
            return true;
        }

        public static void PrintFactura(Factura f, IFacturaInteractor? i)
        {
            if (GetEstation?.ResolveDriver() is { } d)
                d.PrintFactura(f, i);
            else
                MessageTarget?.Warning("La estación de facturación no tiene una impresora configurada.");
        }
    }
}
