using System;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Define un controlador de impresora de facturas.
    /// </summary>
    public abstract class FacturaPrintDriver : Plugin, IExposeGuid
    {
        private readonly IExposeGuid _implementor;

        /// <Inheritdoc/>
        public Guid Guid => _implementor.Guid;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FacturaPrintDriver"/>.
        /// </summary>
        protected FacturaPrintDriver()
        {
            _implementor = new ExposeGuidImplementor(this);
        }

        /// <summary>
        /// Imprime una factura.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="i"></param>
        public abstract void PrintFactura(Factura f, IFacturaInteractor? i);

        /// <summary>
        /// Imprime una pre-factura.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="i"></param>
        public abstract void PrintProforma(Factura f, IFacturaInteractor? i);
    }
}
