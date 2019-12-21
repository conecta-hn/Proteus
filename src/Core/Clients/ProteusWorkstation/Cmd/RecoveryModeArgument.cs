/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;
using TheXDS.Proteus.ViewModels;
using System.Collections.Generic;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.Modules;
using System.Threading.Tasks;
using System.Linq;
using TheXDS.Proteus.Pages;

namespace TheXDS.Proteus.Cmd
{
    public class RecoveryModeArgument : AdministrativeArgument
    {
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

            App.UiInvoke(Vm.OpenMainPage);
            Vm.UpdateStatus(100, $"Inicialización completada, {App.Info.Name} se iniciará en unos segundos.");
            Vm.UpdateStatus(100, $"Se escribirá una bitácora en {logger.Logger?.LogFile}");
            await Task.Delay(10000);
            Proteus.CommonReporter?.Done();
        }
    }
}