using System;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Facturacion.Component;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Facturacion.Pages;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.Widgets;

[assembly: Name("Módulo de gestión de datos de Conect@")]

namespace TheXDS.Proteus.Modules
{
    [Name("Conect@")]
    public class ConectaModule : UiModule
    {
        public static Launcher DefaultFacturaLauncher { get; private set; }

        public IFacturaUIInteractor Interactor { get; private set; }

        private void RegisterInteractor(Type t)
        {
            Interactor = t?.New<IFacturaUIInteractor>();
            var m = new SimpleCommand(f =>
            {
                App.RootHost.OpenPage(new FacturadorPage(f as Factura, Interactor));
            });

            var launcher = new Launcher(t is null ? "Facturación" : $"Facturación - {t?.NameOf()}", null, $"{typeof(ConectaModule).FullName}<{t?.FullName}>", m);
            DefaultFacturaLauncher = launcher;
            RegisterLauncher(launcher, InteractionType.Operation.NameOf());
            Essentials.Add(launcher);
        }

        protected override void AfterInitialization()
        {
            AutoRegisterMenu<ConectaService>();
        }
    }
}