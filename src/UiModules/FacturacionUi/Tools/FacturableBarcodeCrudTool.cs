/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using BarcodeLib;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.Models;
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
            yield return Launcher.FromMethod(
                "Generar código de barras",
                $"Permite generar un código de barras para este ítem.",
                GenBarcode, () => vm);

            yield return Launcher.FromMethod(
                "Clonar y editar",
                "Crea una copia de este ítem, y permite editarlo. Útil para crear nuevos SKU de ítems similares.",
                GenNewItem, ()=> vm);
        }

        private void GenNewItem(ICrudViewModel vm)
        {
            var entity = (Facturable)vm!.Selection!;
            var copy = entity.GetType().New<Facturable>();
            CopyInfo(entity, copy);
            vm.CreateNewFrom(copy);
        }

        private static void CopyInfo(Facturable entity, Facturable copy)
        {
            copy.Name = entity.Name;
            copy.Isv = entity.Isv;
            copy.Precio = entity.Precio;
            copy.Category = entity.Category;
            if (entity is Producto p1 && copy is Producto p2)
            {
                p2.Description = p1.Description;
                p2.Picture = p1.Picture;
                p2.StockMin = p1.StockMin;
                p2.StockMax = p1.StockMax;
                p2.ExpiryDays = p1.ExpiryDays;
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

    public class FacturableSkuCrudTool : CrudTool<Facturable>
    {
        public FacturableSkuCrudTool() : base(CrudToolVisibility.Editing)
        {
        }
        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel? vm)
        {
            yield break;
            foreach (var j in models)
            {
                yield return Launcher.FromMethod(
                    "Generar SKU",
                    $"Permite generar un nuevo SKU para el {CrudElement.GetDescription(j)?.FriendlyName ?? j.Name}.",
                    GenSku, () => vm);
            }
        }

        private static string GenNewId(byte len = 11)
        {
            var rnd = new Random();
            var buff = new byte[8];
            rnd.NextBytes(buff);
            var c = BitConverter.ToInt64(buff, 0) & long.MaxValue;
            return c.ToString().PadLeft(len, '0').Substring(0, len);
        }

        private void GenSku(ICrudViewModel? vm)
        {
            var tries = 0;
            byte len = 11;
            if (vm!.Selection!.IsNew)
            {
                do
                {
                    if (++tries % 10 == 0) len++;
                    vm!.Selection!.Id = GenNewId(len);
                } while (Proteus.Service<FacturaService>()!.Exists(vm!.Selection!.GetType(),vm!.Selection!.StringId));
                (vm as NotifyPropertyChangeBase)?.Notify("Id");
            }
            else
            {
                Proteus.MessageTarget?.Stop("No se puede generar un nuevo SKU para un ítem que ya existe.");
            }
            return;
        }
    }
}
