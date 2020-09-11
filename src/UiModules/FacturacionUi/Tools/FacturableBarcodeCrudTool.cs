/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using BarcodeLib;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.Proteus.FacturacionUi.Lib.BarcodeHelper;

namespace TheXDS.Proteus.FacturacionUi.Tools
{
    public class FacturableBarcodeCrudTool : CrudTool<Facturable>
    {
        public FacturableBarcodeCrudTool() : base(CrudToolVisibility.Selected)
        {
        }

        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel? vm)
        {
            foreach (var j in models)
            {
                yield return Launcher.FromMethod(
                    "Generar código de barras",
                    $"Permite generar un código de barras para este tipo de {CrudElement.GetDescription(j)?.FriendlyName ?? j.Name}.",
                    GenBarcode, () => vm);
            }
        }

        private void GenBarcode(ICrudViewModel? vm)
        {
            var m = TYPE.CODE128B;
            if (!InputSplash.Get("Seleccione el tipo de código de barra a generar", ref m)) return;
            try
            {
                var pd = new PrintDialog();
                if (pd.ShowDialog() ?? false)
                {
                    pd.PrintVisual(RenderBarcode(m, vm!.Selection!.StringId), $"Código de barras {m.NameOf()} para {vm!.SelectedElement!.Description.FriendlyName} {vm!.Selection!.StringId} - {App.Info.Name}");
                }
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Warning($"No se pudo utilizar el tipo {m.NameOf()} para generar el código de barra: {ex.Message}");
            }
        }
    }
}
