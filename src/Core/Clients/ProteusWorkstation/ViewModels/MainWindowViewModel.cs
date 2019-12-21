/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Threading.Tasks;
using System.Windows;
using TheXDS.MCART.Component;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Pages;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.Types.Extensions.StringExtensions;
using static TheXDS.Proteus.ViewModels.LoginViewModel;

namespace TheXDS.Proteus.ViewModels
{
    /// <summary>
    ///     ViewModel que controla el comportamiento de la ventana principal de
    ///     la aplicación.
    /// </summary>
    public class MainWindowViewModel : ReportingPageHostViewModel, IRootPageHost
    {
        private readonly IPageRootVisualHost _rootHost;

        /// <summary>
        ///     Obtiene el nivel de opacidad configurado para la ventana.
        /// </summary>
        public double Opacity => Settings.Default.UiOpacity;

        /// <summary>
        ///     Determina si el logo de la aplicación debería ser visible al
        ///     fondo de la ventana principal.
        /// </summary>
        public bool IsLogoVisible => Opacity > MinLogoOpacity;

        /// <summary>
        ///     Obtiene el nivel de opacidad de la capa de ruido del fondo de
        ///     la aplicación.
        /// </summary>
        public double NoiseUI => Settings.Default.NoiseUI;

        /// <summary>
        ///     Obtiene el nivel de opacidad a utilizar para el logotipo de la
        ///     aplicación.
        /// </summary>
        public double LogoOpacity => ((Opacity - MinLogoOpacity) / (1 - MinLogoOpacity)).Clamp(0, 1);

        /// <summary>
        ///     Obtiene el nivel de opacidad en el cual el logo deja de ser
        ///     visible al fondo de la ventana de la aplicación.
        /// </summary>
        public double MinLogoOpacity => Settings.Default.MinUiOpacity;

        /// <summary>
        ///     Obtiene la UI a utilizar para mostrar avances de estado.
        /// </summary>
        public FrameworkElement? ReporterUi
        {
            get
            {
                return Settings.Default.WindowUiMode switch
                {
                    UiMode.Simple => new SimpleUiMode(),
                    UiMode.Flat => new FlatUiMode(),
                    UiMode.Minimal => new MinimalUiMode(),
                    UiMode.Logging => new LoggingUiMode(this),
                    _ => null
                };
            }
        }

        /// <summary>
        ///     Comando de cierre de sesión.
        /// </summary>
        public SimpleCommand LogoutCommand { get; }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="MainWindowViewModel"/>.
        /// </summary>
        /// <param name="window">
        ///     Host visual de este ViewModel. Debe tratarse de la ventana
        ///     principal de la aplicación.
        /// </param>
        public MainWindowViewModel(IPageRootVisualHost window) : base(window, true)
        {
            _rootHost = window;
            Title = App.Info.Name;
            LogoutCommand = new SimpleCommand(Logout);
            var s = Settings.Default;
            if (!(s is null)) s.PropertyChanged += Default_PropertyChanged;
        }

        private void EarlySetup()
        {
            ViewModelFactory.AttributeExclusionList.Add(typeof(ExcludeFromVmFactoryAttribute));
            Proteus.MessageTarget = new MessageSplashTarget();
            Proteus.CommonReporter = this;
            App.RootHost = this;

            var args = new CmdLineParser();
            foreach (var j in args.Present) j.Run(args);
        }

        internal static bool _exiting = false;

        /// <summary>
        ///     Inicia la aplicación de forma asíncrona.
        /// </summary>
        /// <returns>
        ///     Una tarea que puede utilizarse para monitorear la operación.
        /// </returns>
        public Task LaunchAsync()
        {
            EarlySetup();

            if (_exiting)
            {
                return Task.CompletedTask;
            }

            if (!Settings.Default.Launched)
            {
                App.UiInvoke(OpenPage<SettingsPage>);
                return Task.CompletedTask;
            }

            return PostSettingsInit();
        }

        internal async Task PostSettingsInit()
        {
            UpdateStatus("Preparando aplicación...");

            await Proteus.Init(Settings.Default);
            Proteus.LogoutActions.Add(Logout);

            UpdateStatus("Cargando componentes...");
            await App.LoadPlugins();

            UpdateStatus("Pre-construyendo elementos de UI...");
            await Task.Run(CrudElement.Preload);

            Done();
            if (App.KickStarter.RequiresInteractiveLogin)
            {
                Proteus.Interactive = true;
                App.UiInvoke(Logout);
            }

            else
            {
                App.UiInvoke(OpenMainPage);
            }
        }

        private void Logout()
        {
            if (!Proteus.Interactive) return;
            App.UiInvoke(() =>
            {
                Pages.Clear();
                var lp = App.KickStarter.GetLoginPage() ?? App.FromKickStarter(p=>p.GetLoginPage());
                lp.ViewModel.LoginSuccess += ViewModel_LoginSuccess;
                OpenPage(lp);
            });
        }

        private void ViewModel_LoginSuccess(object? sender, LoginSuccessEventArgs e)
        {
            if (Settings.Default.RememberLastLogin)
            {
                Settings.Default.LastLogin = e.Credential.Username;
                if (Settings.Default.RememberPassword)
                    Settings.Default.SavedPassword = e.Credential.Password.Read();
                Settings.Default.Save();
            }
            OpenMainPage();
        }

        /// <summary>
        ///     Abre la página principal de la aplicación.
        /// </summary>
        public void OpenMainPage()
        {
            Title = $"{Proteus.Session?.Name.OrNull("{0} - ")}Proteus";
            OpenPage(App.KickStarter.GetMainPage() ?? App.FromKickStarter(p => p.GetMainPage()));
        }

        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.UiOpacity):
                    Notify(new[]
                    {
                        nameof(Opacity),
                        nameof(IsLogoVisible),
                        nameof(LogoOpacity),
                    });
                    break;
                case nameof(Settings.MinUiOpacity):
                    Notify(new[]
                    {
                        nameof(MinLogoOpacity),
                        nameof(IsLogoVisible),
                        nameof(LogoOpacity),
                    });
                    break;
                case nameof(Settings.NoiseUI):
                    Notify(nameof(NoiseUI));
                    break;
                case nameof(Settings.MainWindowUiMode):
                case nameof(Settings.WindowUiMode):
                    Notify(nameof(ReporterUi));
                    break;
            }
        }

        /// <summary>
        ///     Fureza el cierre de la aplicación.
        /// </summary>
        public void ForceClose()
        {
            _rootHost.ForceClose();
        }
    }
}