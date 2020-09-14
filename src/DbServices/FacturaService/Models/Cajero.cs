using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Representa a un cajero en el sistema.
    /// </summary>
    public class Cajero : User<int>
    {
        /// <summary>
        /// Balance de apertura de caja preferido.
        /// </summary>
        public decimal OptimBalance { get; set; }

        /// <summary>
        /// Colección de sesiones de caja abiertas por el cajero.
        /// </summary>
        public virtual List<CajaOp> Sesiones { get; set; } = new List<CajaOp>();
    }
}