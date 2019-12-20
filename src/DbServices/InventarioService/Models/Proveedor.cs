using System;
using System.Collections.Generic;
using System.Data.Entity;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Conecta.Context;
using TheXDS.Proteus.Conecta.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Conecta
{
    namespace Models
    {
        public class Item : Nameable<long>, ITimestamp, IPagable
        {
            public string? NumSerie { get; set; }
            public virtual Proveedor? Proveedor { get; set; }
            public string Description { get; set; }
            public virtual List<ItemPicture> Pictures { get; set; } = new List<ItemPicture>();
            public DateTime Timestamp { get; set; }
            public int Qty { get; set; }
            public decimal Total { get; set; }
            public decimal? UnitVenta { get; set; }

            public virtual List<Pago> Pagos { get; set; } = new List<Pago>();
        }

        public class ItemPicture : ModelBase<long>
        {
            public string Path { get; set; }
            public string Notes { get; set; }
        }

        public class Proveedor : Contact<int>
        {
            public virtual List<Item> Inventario { get; set; } = new List<Item>();
        }

        public class Compra : TimestampModel<long>, IPagable
        {
            public virtual Cliente Comprador { get; set; }
            public virtual Item Item { get; set; }
            public int Qty { get; set; }
            public decimal Total { get; set; }
            public virtual List<Pago> Pagos { get; set; } = new List<Pago>();
        }

        public class Cliente : Contact<int>
        {
            public virtual List<Compra> Compras { get; set; } = new List<Compra>();
        }

        public class Pago : TimestampModel<long>
        {
            public decimal Abono { get; set; }

            public override string ToString()
            {
                return $"Pago por {Abono:C}";
            }
        }
    }

    namespace Context
    {
        [DbConfigurationType(typeof(DbConfig))]
        public class ConectaContext : DbContext
        {
            public DbSet<Item> Items { get; set; }
            public DbSet<ItemPicture> Pictures { get; set; }
            public DbSet<Proveedor> Proveedores { get; set; }
            public DbSet<Compra> Compras { get; set; }
            public DbSet<Cliente> Clientes { get; set; }
            public DbSet<Pago> Pagos { get; set; }
        }
    }

    namespace Api
    {
        public class ConectaService : Service<ConectaContext>
        {
        }
    }
}
