/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Facturacion.Models;
using System;
using System.Collections.Generic;

namespace TheXDS.Proteus.Facturacion.Component
{
    public class FacturaColumn
    {
        public string Header { get; }

        public Func<ItemFactura, string> Presenter { get; }

        public double RelaSize { get; }
        public bool Currency { get; }

        public FacturaColumn(string header, Func<ItemFactura, string> presenter) : this(header, presenter, 1.0)
        {
        }
        public FacturaColumn(string header, Func<ItemFactura, string> presenter, double relaSize) : this(header, presenter, relaSize, false)
        {
        }

        public FacturaColumn(string header, Func<ItemFactura, string> presenter, double relaSize, bool currency)
        {
            Header = header;
            Presenter = presenter;
            RelaSize = relaSize;
            Currency = currency;
        }
    }

    public interface IFacturaInteractor
    {
        IEnumerable<FacturaColumn> ExtraColumns { get; }

        /// <summary>
        ///     Informa al <see cref="IFacturaInteractor"/> que la facturación 
        ///     se ha realizado.
        /// </summary>
        System.Threading.Tasks.Task OnFacturate();
    }
}
