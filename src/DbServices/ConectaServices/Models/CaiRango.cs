/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Facturacion.Models
{
    /// <summary>
    ///     Modelo que describe un rango de facturación.
    /// </summary>
    public class CaiRango : ModelBase<int>
    {
        /// <summary>
        ///     CAI padre de este rango de facturación.
        /// </summary>
        public virtual Cai Parent { get; set; }

        /// <summary>
        ///     Número de local del rango aoturizado.
        /// </summary>
        public short NumLocal { get; set; } = 1;

        /// <summary>
        ///     Número de documento expedible con este rango de facturación.
        /// </summary>
        public byte NumDocumento { get; set; } = 1;

        /// <summary>
        ///     Número de caja aprobado para este rango de facturación.
        /// </summary>
        public short NumCaja { get; set; } = 1;

        /// <summary>
        ///     Correlativo de factura inicial del rango de facturación.
        /// </summary>
        public int RangoInicial { get; set; } = 1;

        /// <summary>
        ///     Correlativo de factura final del rango de facturación.
        /// </summary>
        public int RangoFinal { get; set; } = 100;

        /// <summary>
        ///     Contiene todas las facturas creadas dentro de este rango de
        ///     facturación.
        /// </summary>
        public virtual List<Factura> Facturas { get; set; } = new List<Factura>();


        public override string ToString()
        {
            return $"{Prefix()}, de {RangoInicial:00000000} a {RangoFinal:00000000}";
        }

        /// <summary>
        ///     Obtiene el número de factura correspondiente al correlativo
        ///     indicado.
        /// </summary>
        /// <param name="correl">
        ///     Correlativo para el cual obtener un número de factura.
        /// </param>
        /// <returns>El número completo de factura.</returns>
        public string FactNum(int correl)
        {
            return $"{Prefix()}-{RangoInicial+correl-1:00000000}";
        }

        private string Prefix()
        {
            return $"{NumLocal:000}-{NumCaja:000}-{NumDocumento:00}";
        }
    }
}
