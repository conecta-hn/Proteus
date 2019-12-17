/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TheXDS.MCART;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Pages;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.Types.Extensions.StringExtensions;

namespace TheXDS.Proteus.ViewModels
{
    /// <summary>
    ///     ViewModel que gestiona el comportamiento de la página de inicio de
    ///     la aplicación.
    /// </summary>
    [DebuggerStepThrough]
    public class HomeViewModel : PageViewModel, IAlertTarget
    {
        public string UserGreeting => Proteus.Interactive ? $"Hola, {Proteus.Session?.Name.Split()[0] ?? "usuario"}." : "Inicio";
        private readonly ObservableCollection<Alerta> _alertas = new ObservableCollection<Alerta>();
        private DateTime _dayOfCalendar;
        private readonly HashSet<ModulePage> _modules;

        public Visibility Interactive => Proteus.Interactive ? Visibility.Visible : Visibility.Collapsed;
        public IEnumerable<Aviso> Avisos => UserService.AllAvisos.ToList();
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
                foreach (var j in _modules)
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

        public IEnumerable<ModulePageViewModel> ModuleMenus => _modules.Where(p => p.Module.HasEssentials).Select(p => p.ViewModel);

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
            _modules = new HashSet<ModulePage>(App.Modules?.Select(p =>new ModulePage(p)));
            Settings.Default.PropertyChanged += Default_PropertyChanged;

            LogoutCommand = new SimpleCommand(Proteus.Logout);
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
            base.Close();
            Proteus.Logout();
        }

        public double UiModulesHeight => Settings.Default.UiModulesHeight;
        public double UiModulesWidth => Settings.Default.UiModulesWidth;
        public ICommand LogoutCommand { get; }
    }
}