/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.ReflectionHelpers;
using TheXDS.MCART;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.FacturacionUi.Modules;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Pages;

namespace TheXDS.Proteus.Plugins
{
    public class OrdenTrabajoCrudTool : CrudTool<OrdenTrabajo>
    {
        public OrdenTrabajoCrudTool() : base(CrudToolVisibility.Selected)
        {
        }

        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel? vm)
        {
            yield return new Launcher(
                "Imprimir orden",
                "Permite imprimir una copia de la orden de trabajo.",
                GetMethod<OrdenTrabajoCrudTool, Action<OrdenTrabajo, ICrudViewModel>>(p => p.OnPrint).FullName(),
                new SimpleCommand(() => OnPrint((OrdenTrabajo)vm!.Selection!, vm)), null);
            yield return new Launcher(
                "Facturar orden",
                "Permite facturar la orden de trabajo.",
                GetMethod<OrdenTrabajoCrudTool, Action<OrdenTrabajo, ICrudViewModel>>(p => p.OnFacturar).FullName(),
                new SimpleCommand(() => OnFacturar((OrdenTrabajo)vm!.Selection!, vm)), null);
        }

        private async void OnFacturar(OrdenTrabajo obj, ICrudViewModel? vm)
        {
            Factura f;
            if (obj.Facturas.FirstOrDefault() is { } ff)
            {
                f = ff;
            }
            else
            {
                f = new Factura() { Cliente = obj.Cliente };
                foreach (var j in obj.Items)
                {
                    new ItemFactura
                    {
                        Item = j.Item,
                        Qty = j.Qty,
                        StaticPrecio = j.Item.Precio,
                        StaticIsv = !obj.Cliente.AnyExoneraciones() ? j.Item.Isv : null
                    }.PushInto(f.Items);
                }
            }
            obj.Facturado = true;
            App.Module<FacturacionModule>()!.Host.OpenPage(new FacturadorPage(f));
        }

        private void OnPrint(OrdenTrabajo obj, ICrudViewModel? vm)
        {
            //if (obj.IsNew)
            //{
            //    Proteus.MessageTarget?.Stop("La orden debe guardarse primero.");
            //    return;
            //}
            FacturaService.PrintOt(obj);
            //vm?.SaveCommand.Execute(obj);
        }
    }
}
