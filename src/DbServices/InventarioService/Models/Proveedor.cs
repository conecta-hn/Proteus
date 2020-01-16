using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Conecta.Context;
using TheXDS.Proteus.Conecta.Models;
using TheXDS.Proteus.Context;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Conecta
{
    namespace Models
    {
        // Inventario
        public class Item : Nameable<long>
        {
            public virtual Lote Parent { get; set; }
            public virtual Menudeo? MenudeoParent { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public decimal? Descuento { get; set; }
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{Parent?.Name.OrNull("{0} ")}{Name.OrNull("({0})") ?? StringId}");
                if (MenudeoParent is IPagable m)
                {
                    sb.Append(m.Debe == 0 ? " (vendido)" : $" (crédito a {MenudeoParent.Vendedor})");
                }


                return sb.ToString();
            }
            public string? Info => Parent?.Name;
        }
        public class Lote : Nameable<long>, ITimestamp, IDescriptible
        {
            public virtual Proveedor? Proveedor { get; set; }
            public virtual List<Inversion> Inversion { get; set; } = new List<Inversion>();
            public string Description { get; set; }
            public virtual List<Item> Items { get; set; } = new List<Item>();
            public virtual List<ItemPicture> Pictures { get; set; } = new List<ItemPicture>();
            public DateTime Timestamp { get; set; }
            public decimal? UnitVenta { get; set; }
        }
        public class ItemPicture : ModelBase<long>
        {
            public string Path { get; set; }
            public string Notes { get; set; }
        }
        public class Inversion : TimestampModel<long>, IPagable
        {
            public virtual Lote Item { get; set; }
            public virtual Inversor Inversor { get; set; }
            public decimal Total { get; set; }
            public virtual List<Pago> Pagos { get; set; } = new List<Pago>();

            public override string ToString()
            {
                var pagado = ((IPagable)this).Pagado;
                return $"{Inversor?.Name ?? "N/A"} con {Total:C}{(pagado < Total ? $", se le debe {Total - pagado:C}" : null)}";
            }
        }
        public class Pago : TimestampModel<long>
        {
            public decimal Abono { get; set; }

            public override string ToString()
            {
                return $"Pago por {Abono:C}";
            }
        }
        public class Proveedor : Contact<int>
        {
            public virtual List<Lote> Inventario { get; set; } = new List<Lote>();
        }
        public class Inversor : Contact<int>
        {
            public virtual List<Inversion> Inversion { get; set; } = new List<Inversion>();

            public decimal InvTotal => Inversion.Any() ? Inversion.Sum(p => p.Total) : 0m;
        }
        public class Vendedor : Contact<int>
        {
            public virtual List<Menudeo> Items { get; set; } = new List<Menudeo>();
        }
        public class Menudeo : TimestampModel<long>, IPagable
        {
            public virtual Vendedor Vendedor { get; set; }
            public virtual List<Item> Items { get; set; } = new List<Item>();
            public decimal Total { get; set; }
            public virtual List<Pago> Pagos { get; set; } = new List<Pago>();
            public override string ToString()
            {
                var pagado = ((IPagable)this).Pagado;
                return $"{Vendedor?.Name ?? "N/A"} por {Total:C}{(pagado < Total ? $", debe {Total - pagado:C}" : null)}";
            }
        }


        // Gestión de actividades
        public class Actividad : TimestampModel<int>, INameable, IDescriptible, IVoidable
        {
            public DateTime? Void { get; set; }
            public string Name { get; set; }
            public virtual List<ActividadItem> Items { get; set; } = new List<ActividadItem>();
            public string? Description { get; set; }
        }
        public class ActividadItem : Nameable<int>, IDescriptible
        {
            public decimal RawValue { get; set; }
            public string? Description { get; set; }
        }
        public class Asistencia : TimestampModel<int>, IVoidable
        {
            public virtual Empleado Empleado { get; set; }
            public DateTime? Void { get; set; }
        }
        public class Empleado : Contact<int>
        {
        }
    }

    namespace Context
    {
        public class ConectaContext : ProteusContext
        {
            public DbSet<Item> Items { get; set; }
            public DbSet<Lote> Lotes { get; set; }
            public DbSet<ItemPicture> Pictures { get; set; }
            public DbSet<Pago> Pagos { get; set; }
            public DbSet<Proveedor> Proveedores { get; set; }
            public DbSet<Inversor> Inversores { get; set; }
            public DbSet<Inversion> Inversiones { get; set; }
            public DbSet<Menudeo> Menudeos { get; set; }
            public DbSet<Vendedor> Vendedores { get; set; }
        }
        public class ActividadContext : ProteusContext
        {
            public DbSet<Actividad> Actividades { get; set; }
            public DbSet<ActividadItem> ActividadItems { get; set; }
            public DbSet<Asistencia> Asistencias { get; set; }
            public DbSet<Empleado> Empleados { get; set; }
        }
    }

    namespace Api
    {
        public class ConectaService : Service<ConectaContext>
        {
        }
        public class ConectaActService : Service<ActividadContext>
        {
        }
    }
}
