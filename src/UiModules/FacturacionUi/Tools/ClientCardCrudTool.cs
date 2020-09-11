/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using BarcodeLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.FacturacionUi.Tools
{
    public class ClientCardCrudTool : CrudTool<Cliente>
    {
        public ClientCardCrudTool() : base(CrudToolVisibility.Selected)
        {
        }
        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel? vm)
        {
            foreach (var j in models)
            {
                yield return Launcher.FromMethod(
                    "Generar carnet de cliente",
                    $"Genera un carnet para el cliente.",
                    GenCarnet, () => vm);
            }
        }

        private void GenCarnet(ICrudViewModel? vm)
        {
            var m = TYPE.CODE128B;
            try
            {
                var cliente = (Cliente)vm!.Selection!;
                var pd = new PrintDialog();
                if (pd.ShowDialog() ?? false)
                {
                    var v = new DrawingVisual();
                    using var dc = v.RenderOpen();
                    using var barcode = new Barcode
                    {
                        IncludeLabel = true,
                        LabelPosition = LabelPositions.BOTTOMCENTER,
                        ImageFormat = System.Drawing.Imaging.ImageFormat.Png,
                        LabelFont = new System.Drawing.Font("Consolas", 24)
                    };
                    var img = barcode.Encode(TYPE.UPCA, cliente.Id.ToString().PadLeft(11,'0'));
                    dc.DrawImage(img.ToSource(), new Rect 
                    {
                        X=100,
                        Y=100,
                        Width = img.Width,
                        Height = img.Height
                    });
                    
                    dc.DrawText(new FormattedText(
                        cliente.Name,
                        CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        new Typeface("Calibri"), 14, Brushes.Black, 1.0),
                        new Point(50, 50));

                    pd.PrintVisual(v, $"Carnet para {cliente.Name} - {App.Info.Name}");
                }
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Warning($"No se pudo utilizar el tipo {m.NameOf()} para generar el código de barra: {ex.Message}");
            }
        }
    }
}
