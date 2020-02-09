/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.ReflectionHelpers;
using TheXDS.MCART;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.FacturacionUi.Modules;
using System.Threading.Tasks;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Pages;

namespace TheXDS.Proteus.Plugins
{
    public class OrdenTrabajoCrudTool : CrudTool<OrdenTrabajo>
    {
        public OrdenTrabajoCrudTool() : base(CrudToolVisibility.EditAndSelected)
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
            if (obj.IsNew)
            {
                vm?.SaveCommand.Execute(obj);
                await Task.Delay(1000);
            }
            Factura f;
            if (obj.Facturas.FirstOrDefault() is { } ff)
            {
                f = ff.PushInto(obj.Facturas);
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
                        StaticIsv = j.Item.Isv
                    }.PushInto(f.Items);
                }
            }
            App.Module<FacturacionModule>()!.Host.OpenPage(new FacturadorPage(f));
        }

        private void OnPrint(OrdenTrabajo obj, ICrudViewModel? vm)
        {
            if (obj.IsNew)
            {
                Proteus.MessageTarget?.Stop("La orden debe guardarse primero.");
                return;
            }
            FacturaService.PrintOt(obj);
            vm?.SaveCommand.Execute(obj);
        }
    }
    public class FacturaCrudTools : CrudTool<Factura>
    {
        public FacturaCrudTools() : base(CrudToolVisibility.EditAndSelected)
        {
        }

        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel vm)
        {
            yield return new Launcher(
                "Imprimir factura",
                "Permite imprimir una copia de la factura.",
                GetMethod<FacturaCrudTools, Action<Factura>>(p => p.OnPrint).FullName(),
                new SimpleCommand(() => OnPrint(vm.Selection as Factura)), null);
            yield return new Launcher(
                "Anular factura",
                "Permite imprimir una copia de la factura.",
                GetMethod<FacturaCrudTools, Action<ICrudViewModel>>(p => p.OnNullify).FullName(),
                new SimpleCommand(() => OnNullify(vm)), null);
        }

        private void OnPrint(Factura? obj)
        {
            if (obj is null) return;
            if (obj.Impresa)
            {
                FacturaService.PrintFactura(obj, App.Module<FacturacionModule>()?.Interactor);
            }
            else
            {
                FacturaService.AddFactura(obj, true, App.Module<FacturacionModule>()?.Interactor);
            }
        }

        private async void OnNullify(ICrudViewModel? obj)
        {
            if (obj is null) return;
            if (!(obj.Selection is Factura f)) return;
            f.Nula = true;
            obj.SaveCommand.Execute(f);
        }

    }
}
