using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Modelo que describe un rango de facturación.
    /// </summary>
    public class CaiRango : ModelBase<int>
    {
        /// <summary>
        /// CAI padre de este rango de facturación.
        /// </summary>
        public virtual Cai Parent { get; set; } = null!;

        /// <summary>
        /// Número de local del rango aoturizado.
        /// </summary>
        public short NumLocal { get; set; }

        /// <summary>
        /// Número de documento expedible con este rango de facturación.
        /// </summary>
        public byte NumDocumento { get; set; }

        /// <summary>
        /// Número de caja aprobado para este rango de facturación.
        /// </summary>
        public short NumCaja { get; set; }

        /// <summary>
        /// Correlativo de factura inicial del rango de facturación.
        /// </summary>
        public int RangoInicial { get; set; } = 1;

        /// <summary>
        /// Correlativo de factura final del rango de facturación.
        /// </summary>
        public int RangoFinal { get; set; } = 100;

        /// <summary>
        /// Contiene todas las facturas creadas dentro de este rango de
        /// facturación.
        /// </summary>
        public virtual List<Factura> Facturas { get; set; } = new List<Factura>();

        /// <summary>
        /// Referencia a la estación a la cual este rango de facturación
        /// ha sido asignado.
        /// </summary>
        public virtual Estacion? AssignedTo { get; set; }

        /// <summary>
        /// Convierte este rango en su representación como una cadena.
        /// </summary>
        /// <returns>
        /// Una cadena formateada que representa este rango.
        /// </returns>
        public override string ToString()
        {
            return $"{RangoString()} ({(AssignedTo is null ? "Sin asignar" : $"Asignado a {AssignedTo}")})";
        }

        public string RangoString()
        {
            return $"{NumLocal:000}-{NumCaja:000}-{NumDocumento:00}, de {RangoInicial:00000000} a {RangoFinal:00000000}";
        }

        /// <summary>
        /// Obtiene el número de factura correspondiente al correlativo
        /// indicado.
        /// </summary>
        /// <param name="correl">
        /// Correlativo para el cual obtener un número de factura.
        /// </param>
        /// <returns>El número completo de factura.</returns>
        public string FactNum(int correl)
        {
            return $"{NumLocal:000}-{NumCaja:000}-{NumDocumento:00}-{RangoInicial + correl - 1:00000000}";
        }

        public List<int> GetFreeCorrels()
        {
            var l = Common.Sequence(RangoInicial, RangoFinal).ToList();
            foreach (var j in Facturas) l.Remove(j.Correlativo);
            return l;
        }
    }
}