using TheXDS.Proteus.Component;
using System.Data.Entity;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Context
{
    [DbConfigurationType(typeof(DbConfig))]
    public class ConectaContext : DbContext
    {
        public DbSet<Actividad> Actividades { get; set; }
        public DbSet<ActividadItem> ActividadItems { get; set; }
        public DbSet<EquipoDefinition> EquipoDefinitions { get; set; }
        public DbSet<EquipoInstance> EquipoInstances { get; set; }
        public DbSet<EquipoSpecs> SpecsTable { get; set; }
        public DbSet<CpuInventory> Cpus { get; set; }
        public DbSet<GpuInventory> Gpus { get; set; }
        public DbSet<Cai> Cais { get; set; }
        public DbSet<CaiRango> CaiRangos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ClienteCategory> ClienteCategories { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<InventarioConsumo> InventarioConsumos { get; set; }
        public DbSet<ItemFactura> ItemsFactura { get; set; }
        public DbSet<Paquete> Paquetes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<TablaPrecio> TablasPrecio { get; set; }
        public DbSet<TablaPrecioItem> TablaPrecioItems { get; set; }
    }
}
