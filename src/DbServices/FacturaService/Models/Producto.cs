using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;
using System;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Describe un ítem facturable que define un consumo de inventario.
    /// </summary>
    public class Producto : Facturable
    {
        public string? Description { get; set; }
        public string? Picture { get; set; }
        public int? StockMin { get; set; }
        public int? StockMax { get; set; }
        public int? ExpiryDays { get; set; }
        public virtual List<InventarioKind> Labels { get; set; } = new List<InventarioKind>();
    }

    public class InventarioKind : Nameable<int>
    {
    }

    public class Proveedor : Addressable
    {
        public string? WebPage { get; set; }
        public virtual List<ProveedorContacto> Contactos { get; set; } = new List<ProveedorContacto>();
        public virtual List<Producto> Productos { get; set; } = new List<Producto>();
    }

    public class ProveedorContacto : Contact
    {
        public string Cargo { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }

    public class Bodega : Nameable<int>
    {
        public virtual List<Batch> Batches { get; set; } = new List<Batch>();
    }

    public class Batch : TimestampModel<long>
    {
        public virtual Bodega Bodega { get; set; }
        public virtual Producto Item { get; set; }
        public virtual Lote Lote { get; set; }
        public int Qty { get; set; }
    }

    public class Lote : ModelBase<string>
    {
        public DateTime Manufactured { get; set; }
        public virtual List<Batch> OnBatches { get; set; } = new List<Batch>();
    }

}