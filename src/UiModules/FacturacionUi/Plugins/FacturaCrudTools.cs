﻿/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.FacturacionUi.Modules;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Plugins
{
    public class FacturaCrudTools : CrudTool<Factura>
    {
        public FacturaCrudTools() : base(CrudToolVisibility.EditAndSelected)
        {
        }

        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel vm)
        {
            yield return Launcher.FromMethod(
                "Imprimir factura",
                "Permite imprimir una copia de la factura.",
                OnPrint, () => vm.Selection as Factura);

            yield return Launcher.FromMethod("Anular factura", "Permite anular una factura.", OnNullify, vm);
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

        private void OnNullify(ICrudViewModel? obj)
        {
            if (obj is null) return;
            if (!(obj.Selection is Factura f)) return;
            f.Nula = true;
            obj.SaveCommand.Execute(f);
        }
    }
}
