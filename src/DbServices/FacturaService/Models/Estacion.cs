using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

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
    }
}