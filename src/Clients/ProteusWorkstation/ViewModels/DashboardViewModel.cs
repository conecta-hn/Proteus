/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TheXDS.MCART;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Pages;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.Types.Extensions.StringExtensions;

namespace TheXDS.Proteus.ViewModels
{
    /// <summary>
    /// ViewModel que gestiona el comportamiento de la página de inicio de
    /// la aplicación.
    /// </summary>
    [DebuggerStepThrough]
    public class HomeViewModel : PageViewModel, IAlertTarget, IAsyncRefreshable
    {
        public string UserGreeting => Proteus.Interactive ? $"Hola, {Proteus.Session?.Name.Split()[0] ?? "usuario"}." : "Inicio";
        private readonly ObservableCollection<Alerta> _alertas = new ObservableCollection<Alerta>();
        private DateTime _dayOfCalendar = DateTime.Today;
        private List<Aviso> _avisos;
        private readonly HashSet<ModulePage> _modules;

        public Visibility Interactive => Proteus.Interactive ? Visibility.Visible : Visibility.Collapsed;
        public IEnumerable<Aviso> Avisos => _avisos;
        public ICollection<Alerta> Alertas => _alertas;

        public IEnumerable<object> Pendientes => null;
        public IEnumerable<object> Eventos => null;

        public bool AnyTools => ToolLaunchers.Any();

        public IEnumerable<Launcher> ToolLaunchers
        {
            get
            {
                foreach (var j in App.Tools?.SelectMany(p => p.PluginInteractions) ?? Array.Empty<WpfInteractionItem>())
                {
                    
                    yield return new Launcher(
                        j.Text.OrNull() ?? "👆",
                        j.Description,
                        j.Action.Method.FullName(),
                        new SimpleCommand(() => j.Action(this, EventArgs.Empty)));
                }
            }
        }

        public IEnumerable<Launcher> ModuleLaunchers
        {
            get
            {
                foreach (var j in _modules.Where(CanShow))
                {
                    yield return new Launcher(
                        j.Module.Name,
                        j.Module.Description,
                        j.Module.GetType().FullName,
                        new SimpleCommand(() =>
                        {
                            j.ViewModel.IsFullMenuVisible = true;
                            App.RootHost.OpenPage(j);
                        }));
                }
            }
        }

        public IEnumerable<ModulePageViewModel> ModuleMenus => _modules.Where(CanShow).Where(p=>p.Module.HasEssentials).Select(p => p.ViewModel);

        private static bool CanShow(ModulePage module)
        {
            if (Proteus.Interactive)
            {
                if (Proteus.Session.GetDescriptor<ModuleSecurityDescriptor>(module.Module.GetType().FullName!) is { } d)
                {
                    if (!d.Accesible) return false;
                }
                return Proteus.Session.ModuleBehavior?.HasFlag(SecurityBehavior.Visible) ?? false;
            }

            return true;
        }

        public DateTime DayOfCalendar
        {
            get => _dayOfCalendar;
            set
            {
                if (Change(ref _dayOfCalendar, value))
                    OnPropertyChanged(nameof(Eventos));
            }
        }

        public HomeViewModel(ICloseable host) : base(host, true)
        {
            Title = "Página de inicio";
            Proteus.AlertTarget = this;
            AutoHook();
            _modules = new HashSet<ModulePage>(App.Modules?.Select(p =>new ModulePage(p)) ?? Array.Empty<ModulePage>());
            Settings.Default.PropertyChanged += Default_PropertyChanged;
            LogoutCommand = new SimpleCommand(OnLogout);
            BusyOp(RefreshAsync());
        }

        private void OnLogout()
        {
            if (!Settings.Default.ConfirmLogout || Dialogs.MessageSplash.Ask("Cerrar sesión", "Está seguro que desea cerrar sesión?"))
                Proteus.Logout();
        }

        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.UiModulesHeight):
                    Notify(nameof(UiModulesHeight));
                    break;
                case nameof(Settings.UiModulesWidth):
                    Notify(nameof(UiModulesWidth));
                    break;
            }
        }

        public void Alert(string alert)
        {
            Alert(new Alerta(alert, null, null, Alertas, null));
        }

        public void Alert(string header, string body)
        {
            Alert(new Alerta(header, body, null, Alertas, null));
        }

        public void Alert(string alert, Action<Alerta> interaction)
        {
            Alert(new Alerta(alert, null, interaction, Alertas, null));
        }

        public void Alert(string header, string body, Action<Alerta> interaction)
        {
            Alert(new Alerta(header, body, interaction, Alertas, null));
        }

        public void Alert(string alert, Action<Alerta> interaction, object morInfo)
        {
            Alert(new Alerta(alert, null, interaction, Alertas, morInfo));
        }

        public void Alert(string header, string body, Action<Alerta> interaction, object morInfo)
        {
            Alert(new Alerta(header, body, interaction, Alertas, morInfo));
        }

        private void Alert(Alerta alerta)
        {
            App.UiInvoke(() => Alertas.Locked(p => p.Add(alerta)));
        }
        
        public override void Close()
        {
            if (!Settings.Default.ConfirmLogout || Dialogs.MessageSplash.Ask("Cerrar sesión", "Está seguro que desea cerrar sesión?"))
            {
                base.Close();
                Proteus.Logout();
            }
        }

        public async Task RefreshAsync()
        {
            _avisos = await UserService.AllAvisos.ToListAsync();
            Notify(nameof(Avisos));
        }

        public double UiModulesHeight => Settings.Default.UiModulesHeight;
        public double UiModulesWidth => Settings.Default.UiModulesWidth;
        public ICommand LogoutCommand { get; }
    }
}