/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TheXDS.MCART.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Modules;
using TheXDS.Proteus.Pages;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.ViewModels;

namespace TheXDS.Proteus.Cmd
{
    public class RecoveryModeArgument : AdministrativeArgument
    {
        private const string _totalFail = "Error cerrando la aplicación. Para prevenir daños a la información o al equipo, detenga la ejecución de este programa inmediatamente.";
        private MainWindowViewModel Vm => (MainWindowViewModel)App.RootHost;
       
        public override string Summary => $"Inicia {App.Info.Name} en modo de recuperación.";
        
        protected override async void OnRun(CmdLineParser args)
        {
            var logger = args.Present.OfType<LogFileArgument>().FirstOrDefault() ?? new LogFileArgument();
            if (!args.IsPresent<LogFileArgument>()) logger.Run(args);

            MainWindowViewModel._exiting = true;
            Settings.Default.UseAltLauncher = false;
            Settings.Default.RequireNetworkServerSuccess = false;
            Settings.Default.UiOpacity = 1;
            Settings.Default.NoiseUI = 0;
            Settings.Default.WindowUiMode = UiMode.Logging;
            Settings.Default.RememberLastLogin = false;
            Settings.Default.LastLogin = null;
            Settings.Default.ResartRequired = true;


            Proteus.CommonReporter?.UpdateStatus($"Iniciando {App.Info.Name} en modo de recuperación...");
            if (!await Proteus.SafeInit(Settings.Default))
            {
                Proteus.DisposeSettings();
                Settings.Default.Launched = false;
                Proteus.CommonReporter?.Done();
                App.UiInvoke(Vm.OpenPage<SettingsPage>);
                return;
            }

            App._modules = new HashSet<UiModule>();

            if (App.KickStarter.RequiresInteractiveLogin)
            {
                App._modules.Add(new UserModule());
            }

            Proteus.CommonReporter?.UpdateStatus("Preparando hooks de aplicación...");
            Proteus.LogoutActions.Add(Vm.Close);

            Proteus.CommonReporter?.UpdateStatus("Conectando manejador de excepciones...");
            Hook();

            App.UiInvoke(Vm.OpenMainPage);
            Vm.UpdateStatus(100, $"Inicialización completada, {App.Info.Name} se iniciará en unos segundos.");
            Vm.UpdateStatus(100, $"Se escribirá una bitácora en {logger.Logger?.LogFile}");
            await Task.Delay(5000);
            Proteus.CommonReporter?.Done();
        }

        private void Hook()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Application.Current.DispatcherUnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void Fail(Exception? e)
        {
            if (e is null)
            {
                Environment.FailFast(_totalFail);
                return;
            }
            try
            {
                Proteus.MessageTarget?.Critical(e);
#if DEBUG
                if (Debugger.Launch()) Debugger.Break();
                else
#endif
                Application.Current.Dispatcher.Invoke(() => Application.Current?.Shutdown(e.HResult));
            }
            catch (Exception ex)
            {
                try
                { 
                    App.UiInvoke(() => Proteus.MessageTarget?.Critical(_totalFail));
                }
                catch { }
                Environment.FailFast(_totalFail, ex);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Fail(e.Exception);
        }
        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Fail(e.Exception);
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Fail(e.ExceptionObject as Exception);
        }
    }
}