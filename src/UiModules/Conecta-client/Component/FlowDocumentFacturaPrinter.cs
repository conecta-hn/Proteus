/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Facturacion.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using static TheXDS.MCART.Types.Extensions.FlowDocumentExtensions;

namespace TheXDS.Proteus.Facturacion.Component
{
    public class FlowDocumentFacturaPrinter
    {
        public void Print(Factura f)
        {   
            if (f.Impresa)
            {
                Proteus.MessageTarget?.Stop("No es posible reimprimir una factura.");
                return;
            }
            var fd = new FlowDocument()                
                .Title($"Factura {f.FactNum}");

            fd.FontFamily = new FontFamily("Consolas");
            fd.ColumnWidth = 800;


            var tbl = fd.AddTable(new[]
            {
                new KeyValuePair<string, GridLength>("#", new GridLength(1,GridUnitType.Star)),
                new KeyValuePair<string, GridLength>("Item", new GridLength(4,GridUnitType.Star)),
                new KeyValuePair<string, GridLength>("Cantidad", new GridLength(2,GridUnitType.Star)),
                new KeyValuePair<string, GridLength>("Precio", new GridLength(2,GridUnitType.Star))
            });
            foreach (var j in f.Items)
            {
                tbl.AddRow(new[]
                {
                    j.Id.ToString(),
                    j.Item.Name,
                    j.Qty.ToString(),
                    j.StaticPrecio.ToString("C")
                });
            }
            AddFooter(tbl, "Subtotal", f.SubTotal);
            AddFooter(tbl, "Impuestos", f.SubTGravable);
            AddFooter(tbl, "Descuentos", f.Descuentos);
            AddFooter(tbl, "Otros cargos", f.OtrosCargos);
            AddFooter(tbl, "Total a pagar", f.Total);

            fd.Print($"Factura {f.FactNum} - SLM");
        }

        private void AddFooter(Table tbl, string label, decimal value)
        {
            tbl.AddGroup().AddRow()
                .AddCell(3).Text(label).Right().Done<TableCell>().Done<TableRow>()
                .AddCell().Text(value.ToString("C")).Right();
        }
    }
}