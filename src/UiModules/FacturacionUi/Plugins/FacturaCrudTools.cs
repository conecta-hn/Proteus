/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.FacturacionUi.Modules;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.ReflectionHelpers;

namespace TheXDS.Proteus.Plugins
{
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
