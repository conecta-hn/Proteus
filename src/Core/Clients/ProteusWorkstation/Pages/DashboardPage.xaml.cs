/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TheXDS.Proteus.Pages
{
    /// <summary>
    /// Lógica de interacción para DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage
    {
        private bool _loaded;

        public DashboardPage()
        {
            InitializeComponent();
            ViewModel = new HomeViewModel(this);
            Loaded += DashboardPage_Loaded;
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            ViewModel.Refresh();
            _loaded = true;
        }

        private void DudDia_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //LstRecordatorios.ItemsSource = App.Session.CalendarioPersonal.Where(p => p.Timestamp.Date == (DudDia.Value?.Date ?? p.Timestamp.Date));
        }
        private void BtnCalendar_OnClick(object sender, RoutedEventArgs e)
        {
        //    var btn = sender as FrameworkElement;
        //    btn.SetBusy();
        //    App.Host.AddPage(new UserCalendario());
        //    btn.SetDone();
        }
        private async void ChkRecordatorio_Click(object sender, RoutedEventArgs e)
        {
            //await App.LogonService.SaveAsync(sender as FrameworkElement);
            //await App.Host.RefreshAll();
        }
        private void OpenListItemInSplash(object sender, MouseButtonEventArgs e)
        {
            var element =
                (sender as ListViewItem)?.Content as ITitledText ??
                throw new InvalidCastException($"Los elementos de esta lista deben ser {nameof(ITitledText)}");
            Proteus.MessageTarget?.Show(element.Header, element.Body);
        }
        private void Alerta_OnDismiss(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement)?.Tag as Alerta ?? throw new InvalidOperationException()).Dismiss();
        }
        private void Alerta_OnAction(object sender, RoutedEventArgs e)
        {
            var a = (sender as FrameworkElement)?.Tag as Alerta ?? throw new InvalidOperationException();
            if (a.HasInteraction) a.Action?.Invoke(a);
        }

        private void BtnConfig_OnClick(object sender, RoutedEventArgs e)
        {
            App.RootHost.OpenPage<SettingsPage>();
        }
    }
}
