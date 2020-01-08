using System;
using System.Collections.Generic;
using System.Data.Entity;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Conecta.Context;
using TheXDS.Proteus.Conecta.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Conecta
{
    namespace Models
    {
        public class Item : Nameable<long>
        {
            public virtual Lote Parent { get; set; }
            public virtual Menudeo? MenudeoParent { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public decimal? Descuento { get; set; }
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
                return $"{Inversor} con {Total:C}";
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
        }
    }

    namespace Context
    {
        [DbConfigurationType(typeof(DbConfig))]
        public class ConectaContext : DbContext
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
    }

    namespace Api
    {
        public class ConectaService : Service<ConectaContext>
        {
        }
    }
}
