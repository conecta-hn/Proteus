using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Conecta.Context;
using TheXDS.Proteus.Conecta.Inventario.Context;
using TheXDS.Proteus.Conecta.Inventario.Models;
using TheXDS.Proteus.Conecta.Models;
using TheXDS.Proteus.Context;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Conecta
{
    namespace Inventario
    {
        namespace Models
        {
            public class Picture : ModelBase<long>
            {
                public string Path { get; set; }
                public string Notes { get; set; }
            }

            public class Proveedor : Contact<int>
            {
                public virtual List<OrdenCompra> Ordenes { get; set; } = new List<OrdenCompra>();
            }

            public class Categoria : Nameable<short>
            {
                public virtual List<SpecDefinition> Specs { get; set; } = new List<SpecDefinition>();
            }

            public enum SpecValueKind : byte
            {
                [Name("Sí/No")] Bool,
                [Name("Texto")] Text,
                [Name("Número")] Number
            }

            public class SpecDefinition : Nameable<int>
            {
                public SpecValueKind ValueKind { get; set; }
            }

            public class OrdenCompra : TimestampModel<int>
            {
                public virtual Proveedor? Proveedor { get; set; }
                public string? Notes { get; set; }
                public virtual List<Picture> Pictures { get; set; } = new List<Picture>();
                public virtual List<Item> Items { get; set; } = new List<Item>();
            }

            public class UpKeep : TimestampModel<long>
            {
                public virtual Item Item { get; set; }
                public virtual Item Spare { get; set; }


            }

            public class SpecInfo : ModelBase<long>
            {
                public virtual SpecDefinition Spec { get; set; }
                public string? Value { get; set; }
            }

            public class Item : Nameable<long>
            {
                public virtual Categoria? Categoria { get; set; }

                public decimal Costo { get; set; }

                public virtual List<SpecInfo> Specs { get; set; } = new List<SpecInfo>();

                public virtual OrdenCompra Parent { get; set; }

                public string? Description { get; set; }

                public virtual Bodega? Location { get; set; }
                public string? Serie { get; set; }

            }

            public class Bodega : Nameable<short>
            {
                public virtual List<Item> Items { get; set; } = new List<Item>();
            }

            public class Venta : TimestampModel<int>
            {
                public virtual List<VentaItem> Items { get; set; } = new List<VentaItem>();
                public virtual Vendedor? Vendedor { get; set; }
                public virtual Cliente? Cliente { get; set; }
            }

            public class VentaItem : ModelBase<long>
            {
                public virtual Item Item { get; set; }
                public int Qty { get; set; }
                public decimal VentaUnitaria { get; set; }
                public float Isv { get; set; }
                public float? Comision { get; set; }
            }

            public class Vendedor : Contact<int>
            {
                public virtual List<ComisionPago> Comisiones { get; set; }
                public virtual List<Venta> Ventas { get; set; }

            }

            public class ComisionPago : TimestampModel<long>
            {
                public virtual Vendedor Vendedor { get; set; }
                public decimal Abono { get; set; }
            }

            public class Cliente : Addressable<int>, ITimestamp
            {
                public DateTime Timestamp { get; set; }
                public virtual List<Venta> Ventas { get; set; }
            }

            /*
            public class Lote : Nameable<long>, ITimestamp, IDescriptible
            {
                public virtual List<Inversion> Inversion { get; set; } = new List<Inversion>();
                public string Description { get; set; }
                public decimal? UnitVenta { get; set; }
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
            */
        }
        namespace Context
        {
            public class ConectaContext : ProteusContext
            {
                public DbSet<Picture> Pictures { get; set; }
                public DbSet<Proveedor> Proveedors { get; set; }
                public DbSet<Categoria> Categorias { get; set; }
                public DbSet<SpecDefinition> SpecDefinitions { get; set; }
                public DbSet<OrdenCompra> OrdenCompras { get; set; }
                public DbSet<UpKeep> UpKeeps { get; set; }
                public DbSet<SpecInfo> SpecInfos { get; set; }
                public DbSet<Item> Items { get; set; }
                public DbSet<Bodega> Bodegas { get; set; }
                public DbSet<Venta> Ventas { get; set; }
                public DbSet<VentaItem> VentaItems { get; set; }
                public DbSet<Vendedor> Vendedors { get; set; }
                public DbSet<ComisionPago> ComisionPagos { get; set; }
                public DbSet<Cliente> Clientes { get; set; }
            }

        }
        namespace Api
        {
            public class ConectaService : Service<ConectaContext>
            {
            }
        }
    }
    namespace Models
    {
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


        // TrackHN?
        public class Network : ModelBase<int>
        {
            public virtual List<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
        }
        public class Endpoint : Nameable<int>
        {
            public virtual Router Router { get; set; }
            public virtual Network Network { get; set; }

        }
        public class Router : Nameable<int>
        {
            public virtual List<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
            public virtual List<RouteTable> Routes { get; set; } = new List<RouteTable>();
        }
        public class RouteTable : ModelBase<int>
        {
            public Network? Target { get; set; }
            public Endpoint Via { get; set; }
        }
        public class PackageKind : Nameable<int>
        {
            public Guid? PreAutomator { get; set; }
            public Guid? PostAutomator { get; set; }

            public virtual List<Package> Packages { get; set; } = new List<Package>();
        }
        public abstract class Transportable : ModelBase<string>
        {

        }
        public class Package : Transportable
        {
            public PackageKind Kind { get; set; }

        }
        public class Transit : ModelBase<long>
        {

        }



    }

    namespace Context
    {

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
        public class ConectaActService : Service<ActividadContext>
        {
        }
    }
}
