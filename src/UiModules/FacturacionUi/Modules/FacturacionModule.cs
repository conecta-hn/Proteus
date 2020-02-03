﻿using System;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.FacturacionUi.Component;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Pages;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.FacturacionUi.Modules
{
    /// <summary>
    /// Módulo de facturación.
    /// </summary>
    [Name("Módulo de facturación")]
    public class FacturacionModule : UiModule<FacturaService>
    {
        /// <summary>
        /// Obtiene el <see cref="Launcher"/> predeterminado para abrir la
        /// página de facturación.
        /// </summary>
        public static Launcher DefaultFacturaLauncher { get; private set; } = null!;

        /// <summary>
        /// Interactor del módulo de facturación.
        /// </summary>
        public IFacturaUIInteractor? Interactor { get; private set; }

        /// <summary>
        /// Registra un nuevo <see cref="IFacturaUIInteractor"/> en el módulo.
        /// </summary>
        /// <param name="t">
        /// Tipo del <see cref="IFacturaUIInteractor"/> a instanciar para
        /// establecerlo como el interactor del módulo de facturación.
        /// </param>
        private void RegisterInteractor(Type t)
        {
            Interactor = t?.New<IFacturaUIInteractor>();
            var m = new SimpleCommand(async f =>
            {
                if (await Task.Run(() => FacturaService.IsCajaOpOpen))
                {
                    App.RootHost.OpenPage(new FacturadorPage(f as Factura, Interactor));
                }
                else
                {
                    Proteus.MessageTarget?.Stop("La caja está cerrada.");
                }
            });

            var launcher = new Launcher(t is null ? "Facturación" : $"Facturación - {t.NameOf()}", null, $"{typeof(FacturacionModule).FullName}<{t?.FullName}>", m);
            DefaultFacturaLauncher = launcher;
            RegisterLauncher(launcher, InteractionType.Operation.NameOf());
            Essentials.Add(launcher);
        }

        /// <summary>
        /// Registra un nuevo <see cref="IFacturaUIInteractor"/> en el módulo.
        /// </summary>
        /// <typeparam name="T">
        /// <see cref="IFacturaUIInteractor"/> a instanciar para establecerlo
        /// como el interactor del módulo de facturación.
        /// </typeparam>
        public static void RegisterInteractor<T>() where T : IFacturaUIInteractor, new()
        {
            App.Modules.FirstOf<FacturacionModule>().RegisterInteractor(typeof(T));
        }

        /// <summary>
        /// Permite abrir una nueva sesión de caja.
        /// </summary>
        /// <param name="sender">Objeto que ha producido el evento.</param>
        /// <param name="e">Parámetros del evento.</param>
        [InteractionItem, Essential, InteractionType(InteractionType.Operation), Name("Abrir caja")]
        public async void OpenCaja(object sender, EventArgs e)
        {
            if (FacturaService.IsCajaOpOpen)
            {
                Proteus.MessageTarget?.Stop("La caja ya está abierta.");
                return;
            }
            try
            {
                Proteus.CommonReporter?.UpdateStatus("Abriendo sesión de caja...");
                var cajero = FacturaService.GetCajero;
                var estacion = FacturaService.GetEstation;

                if (cajero is null)
                {
                    Proteus.MessageTarget?.Stop("El usuario actual no es un cajero.");
                    return;
                }
                if (estacion is null)
                {
                    Proteus.MessageTarget?.Stop("Esta estación no es una estación de facturación.");
                    return;
                }
                if (FacturaService.CurrentRango is null)
                {
                    Proteus.MessageTarget?.Stop("Esta estación no tiene facturas disponibles.");
                    return;
                }


                var balance = cajero.OptimBalance;

                await Service!.AddAsync(new CajaOp()
                {
                    Estacion = estacion,
                    Cajero = cajero,
                    OpenBalance = balance,
                    Timestamp = DateTime.Now
                });
                Proteus.AlertTarget?.Alert("Caja abierta correctamente.", $"{cajero} ha abierto una sesión de caja en la estación {estacion} con un fondo de {balance:C}. No olvide cerrar la caja al terminar.");
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Critical(ex);
            }
            finally
            {
                Proteus.CommonReporter?.Done();
            }
        }

        /// <summary>
        /// Permite cerrar una sesión de caja abierta.
        /// </summary>
        /// <param name="sender">Objeto que ha producido el evento.</param>
        /// <param name="e">Parámetros del evento.</param>
        [InteractionItem, Essential, InteractionType(InteractionType.Operation), Name("Cerrar caja")]
        public void CloseCaja(object sender, EventArgs e)
        {
            if (!FacturaService.IsCajaOpOpen)
            {
                Proteus.MessageTarget?.Stop("La caja ya está cerrada.");
                return;
            }
            //error intencional de sintáxis.
        }
    }
}
