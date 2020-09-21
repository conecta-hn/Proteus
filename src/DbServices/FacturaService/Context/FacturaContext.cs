using System.Data.Entity;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Context
{
    public class FacturaContext : ProteusContext
    {
        public DbSet<Abono> Abonos { get; set; }
        public DbSet<Cai> Cais { get; set; }
        public DbSet<CaiRango> CaiRangos { get; set; }
        public DbSet<CajaOp> CajaOps { get; set; }
        public DbSet<Cajero> Cajeros { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ClienteCategory> ClienteCategories { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<Entidad> Entidades { get; set; }
        public DbSet<Estacion> Estaciones { get; set; }
        public DbSet<EstacionBodega> EstacionBodegas { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturableCategory> FacturableCategories { get; set; }
        public DbSet<FacturaXCobrar> FacturasXCobrar { get; set; }
        public DbSet<IsvExoneracion> IsvExoneraciones { get; set; }
        public DbSet<IsvExoneracionType> IsvExoneracionTypes { get; set; }
        public DbSet<ItemFactura> ItemFacturas { get; set; }
        public DbSet<Paquete> Paquetes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<TablaPrecio> TablaPrecios { get; set; }
        public DbSet<TablaPrecioItem> TablaPrecioItems { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<ProveedorContacto> ProveedorContactos { get; set; }
        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<GenericBatch> GenericBatches { get; set; }
        public DbSet<SerialBatch> SerialBatches { get; set; }
        public DbSet<SerialNum> SerialNums { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<InventarioKind> InventarioKinds { get; set; }
        public DbSet<InventarioKindItem> InventarioKindItems { get; set; }
        public DbSet<PaqueteItem> PaqueteItems { get; set; }
        public DbSet<ProductoProveedor> ProductoProveedors { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<WarrantyDefinition> WarrantyDefinitions { get; set; }
    }
}