using System;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Describe una columna de facturación personalizada.
    /// </summary>
    public partial class FacturaColumn
    {
        /// <summary>
        /// Encabezado de la factura.
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// Función que obrendrá el valor a presentar para cada ítem.
        /// </summary>
        public Func<ItemFactura, string> Presenter { get; }

        /// <summary>
        /// Valor que define un tamaño relativo para la columna
        /// </summary>
        public double RelaSize { get; }

        /// <summary>
        /// Determina si la columna debería tratarse como una columna de subtotales.
        /// </summary>
        public bool Currency { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="FacturaColumn"/>.
        /// </summary>
        /// <param name="header">Encabezado de la columna.</param>
        /// <param name="presenter">
        /// Función que obtendrá el valor a presentar.
        /// </param>
        public FacturaColumn(string header, Func<ItemFactura, string> presenter) : this(header, presenter, 1.0)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="FacturaColumn"/>.
        /// </summary>
        /// <param name="header">Encabezado de la columna.</param>
        /// <param name="presenter">
        /// Función que obtendrá el valor a presentar.
        /// </param>
        /// <param name="relaSize">
        /// Tamaño relativo de la columna. De forma predeterminada, se
        /// establece en <c>1.0</c>.
        /// </param>
        public FacturaColumn(string header, Func<ItemFactura, string> presenter, double relaSize) : this(header, presenter, relaSize, false)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="FacturaColumn"/>.
        /// </summary>
        /// <param name="header">Encabezado de la columna.</param>
        /// <param name="presenter">
        /// Función que obtendrá el valor a presentar.
        /// </param>
        /// <param name="relaSize">
        /// Tamaño relativo de la columna. De forma predeterminada, se
        /// establece en <c>1.0</c>.
        /// </param>
        /// <param name="currency">
        /// Indica si la columna debe formatearse como una columna de
        /// subtotales.
        /// </param>
        public FacturaColumn(string header, Func<ItemFactura, string> presenter, double relaSize, bool currency)
        {
            Header = header;
            Presenter = presenter;
            RelaSize = relaSize;
            Currency = currency;
        }
    }
}
