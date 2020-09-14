using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;
using System;
using System.Linq;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Plugins;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Representa una estación de facturación en el sistema.
    /// </summary>
    public class Estacion : EstacionBase
    {
        /// <summary>
        /// Colección de rangos de facturación asignados a la estación.
        /// </summary>
        public virtual List<CaiRango> RangosAsignados { get; set; } = new List<CaiRango>();

        /// <summary>
        /// Colección de sesiones abiertas en esta estación.
        /// </summary>
        public virtual List<CajaOp> Sesiones { get; set; } = new List<CajaOp>();

        /// <summary>
        /// Valor de configuración que generará alertas cuando la cantidad de
        /// facturas disponibles sea igual o menor a este valor.
        /// </summary>
        public int? MinFacturasAlert { get; set; }

        /// <summary>
        /// Valor de configuración que habilita o deshabilita la pantalla
        /// secundaria de detalles de facturación.
        /// </summary>
        public byte? SecondScreen { get; set; }

        /// <summary>
        /// Entidad de facturación asociada a esta estación.
        /// </summary>
        public virtual Entidad Entidad { get; set; } = null!;

        /// <summary>
        /// Bodega desde la cual reducir inventario.
        /// </summary>
        public virtual List<EstacionBodega> Bodegas { get; set; } = new List<EstacionBodega>();

        /// <summary>
        /// Impresora asociada a la estación de facturación.
        /// </summary>
        public string? Printer { get; set; }

        /// <summary>
        /// Referencia al <see cref="Guid"/> del <see cref="PaymentSource"/>
        /// utilizado para crear este pago.
        /// </summary>
        public Guid? PrintDriver { get; set; }

        /// <summary>
        /// Obtiene el <see cref="FacturaPrintDriver"/> asociado a esta estación de facturación.
        /// </summary>
        /// <returns></returns>
        public FacturaPrintDriver? ResolveDriver() => FacturaService.FactPrintDrivers.FirstOrDefault(p => p.Guid == PrintDriver);
    }
}