/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.Proteus.Pages.Base;
using TheXDS.Proteus.Resources;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TheXDS.MCART.Component;
using TheXDS.MCART.Dialogs;
using TheXDS.MCART.Pages;

namespace TheXDS.Proteus.Pages
{
    /// <summary>
    /// Lógica de interacción para SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : ProteusPage, IPage
    {
        public IPageHost PageHost { get; set; }

        public SettingsPage()
        {
            InitializeComponent();
            Settings.Default.Host = this;
            DataContext = Settings.Default;            
        }

        private async Task SaveAndApplySettings()
        {
            var mvm = ProteusViewModel.Get<ViewModels.MainWindowViewModel>();
            Close();
            mvm.Pages.Clear();
            Proteus.CommonReporter?.UpdateStatus("Aplicando configuración");
            if (Settings.Default.Launched)
            {

                string? t = null;
                if (Proteus.Interactive) await (Proteus.LogonService?.GenerateToken(true, out t) ?? Task.FromResult(Api.DetailedResult.Ok));
                await Proteus.ReloadSettings(Settings.Default);
                Proteus.Interactive = App.KickStarter.RequiresInteractiveLogin;
                if (!Proteus.Interactive ||(await Proteus.Login(t!)).Success)
                {
                    mvm.OpenMainPage();
                }

                Proteus.CommonReporter?.Done();
            }
            else
            {
                Settings.Default.Launched = true;
                Settings.Default.Save();
                Proteus.DisposeSettings();
                await mvm.PostSettingsInit();
            }
        }

        private void SaveSettingsAndClose()
        {
            Proteus.MessageTarget?.Info($"Reinicie {App.Info.Name} para aplicar los cambios.");
            Settings.Default.Launched = true;
            Settings.Default.Save();
            Application.Current.Shutdown();
        }

        private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Settings.Default.ResartRequired)
            {
                await SaveAndApplySettings();
            }
            else if (!Settings.Default.Launched)
            {
                SaveSettingsAndClose();
            }
            else
            {
                Proteus.MessageTarget?.Info($"Algunos cambios podrían requerir que se reinicie {App.Info.Name}.");
                Close();
            }
        }

        public void Activate() => PageHost?.SwitchTo(this);
        public void Close() => PageHost?.ClosePage(this);

        private void LstPlugins_DblClick(object sender, MouseButtonEventArgs e)
        {
            var t = ((sender as FrameworkElement)?.DataContext as IExposeAssembly)?.Assembly;
            if (t is null) return;
            var i = new ApplicationInfo(t, InferImage(t));
            AboutBox.ShowDialog(i);
        }

        private static UIElement InferImage(Assembly t)
        {
            if (t == typeof(Proteus).Assembly) return Images.Proteus;
            if (t == typeof(App).Assembly) return Images.Logo;
            return Images.Plugin;
        }

        private void BtnTypeInfo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                new Window
                {
                    Content = new TypeDetails(fe.DataContext?.GetType())
                }.ShowDialog();
            }
        }
    }
}