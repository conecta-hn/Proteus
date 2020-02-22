/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

// Los archivos .settings tienen problemas de compatibilidad con C# 8 y las anotaciones de tipos de referencia nulables.
#nullable disable

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using TheXDS.MCART;
using TheXDS.MCART.Types;
using TheXDS.MCART.ViewModel;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.Resources;
using TheXDS.MCART.Dialogs;

namespace TheXDS.Proteus.Config
{
    public sealed partial class Settings : IPageViewModel, ISettings, INotifyPropertyChanged
    {
        private bool _unsavedChanges;
        private ISettingsRepository _selectedRepo;

        public bool UnsavedChanges
        {
            get => _unsavedChanges || !Launched;
            set
            {
                if (_unsavedChanges == value) return;
                _unsavedChanges = value;
                SaveCommand.SetCanExecute(value || !Launched);
                CancelCommand.SetCanExecute(value && Launched);
                base.OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(UnsavedChanges)));
            }
        }
        public bool Closeable => Launched;
        public Settings()
        {
            SettingsLoaded += Settings_SettingsLoaded;
            SettingsSaving += Settings_SettingsSaving;
            CancelCommand = new SimpleCommand(() => { Reload(); UnsavedChanges = false; }, false);
            CloseCommand = new SimpleCommand(Close, Launched);
            SaveCommand = new SimpleCommand(Save, !Launched);
            if (!Launched) Upgrade();
        }
        private void Settings_SettingsSaving(object sender, CancelEventArgs e)
        {
            UnsavedChanges = false;
        }
        private void Settings_SettingsLoaded(object sender, SettingsLoadedEventArgs e)
        {
            UnsavedChanges = false;
        }
        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UnsavedChanges = true;
            base.OnPropertyChanged(sender, e);
        }
        public void Close()
        {
            Host?.Close();
            Host = null;
        }

        /// <summary>
        /// Obtiene el comando relacionado a la acción AboutMCART.
        /// </summary>
        /// <returns>El comando AboutMCART.</returns>
        public SimpleCommand AboutMCARTCommand { get; } = new SimpleCommand(OnAboutMCART);

        private static void OnAboutMCART()
        {
            WpfRtInfo.Show();
        }

        /// <summary>
        /// Obtiene el comando relacionado a la acción PluginInfo.
        /// </summary>
        /// <returns>El comando PluginInfo.</returns>
        public SimpleCommand PluginInfoCommand { get; } = new SimpleCommand(OnPluginInfo);

        private static void OnPluginInfo()
        {
            if (Proteus.Initialized) new PluginBrowser().ShowDialog();
        }

        public SimpleCommand SaveCommand { get; }
        public SimpleCommand CancelCommand { get; }
        public SimpleCommand CloseCommand { get; }
        public ICloseable Host { get; internal set; }
        public string Title
        {
            get => Closeable ? "Configuración" : $"Configuración incial de {App.Info.Name}";
            set => throw new InvalidOperationException();
        }

        public string Name => App.Info.Name;
        public Version Version => App.Info.Version;
        public string ShortVersion => Version.ToString(2);
        public string InfVersion => App.Info.InformationalVersion;
        public string Description => App.Info.Description;
        public string Copyright => App.Info.Copyright;

        public Proteus.InitMode InitMode
        {
            get => (Proteus.InitMode)ProteusInitMode;
            set => ProteusInitMode = (byte)value;
        }

        public UiMode WindowUiMode
        {
            get => (UiMode)MainWindowUiMode;
            set => MainWindowUiMode = (byte)value;
        }

        public IEnumerable<NamedObject<Proteus.InitMode>> InitModes { get; } = NamedObject<Proteus.InitMode>.FromEnum();
        public IEnumerable<NamedObject<UiMode>> UiModes { get; } = NamedObject<UiMode>.FromEnum();

        public IEnumerable<Service> Services => Proteus.Services;

        public static T Get<T>([CallerMemberName]string caller = null)
        {
            return (T)Default[caller];
        }

        public void Refresh() => Reload();

        public IEnumerable<ISettingsRepository> Repos => Proteus.SettingsRepositories;
        public IEnumerable<IKickStarter> KickStarters => Objects.FindAllObjects<IKickStarter>().ToList();

        public IKickStarter SelectedKickStarter
        {
            get => KickStarters.FirstOrDefault(p => p.GetType().Name == AltLauncher);
            set
            {
                AltLauncher = value?.GetType().Name;
            }
        }

        public ISettingsRepository SelectedRepo
        {
            get => _selectedRepo;
            set
            {
                _selectedRepo = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedRepo)));
            }
        }

        public bool IsAdmin => !Proteus.Interactive || Proteus.Session is null || (Service.CanRunService(GetType().FullName, SecurityFlags.Admin, Proteus.Session) ?? false);

        public IEnumerable<NamedObject<object>> ProteusProps
        {
            get
            {
                foreach (var j in typeof(Proteus).GetProperties().Where(p => p.CanRead))
                {
                    yield return new NamedObject<object>(j.GetValue(null), j.NameOf());
                }
            }
        }
    }
}