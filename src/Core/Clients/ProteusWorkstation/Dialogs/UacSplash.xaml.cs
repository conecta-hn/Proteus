/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels;
using SourceChord.FluentWPF;
using System.Windows;
using System.Windows.Controls;

namespace TheXDS.Proteus.Dialogs
{
    /// <summary>
    /// Lógica de interacción para UacSplash.xaml
    /// </summary>
    public partial class UacSplash : AcrylicWindow, ICloseable
    {
        private LoginViewModel Vm => DataContext as LoginViewModel;
        private UacSplash() : this(Proteus.Session)
        {            
        }

        private UacSplash(IProteusCredential credential)
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this, new Credential(credential.Id))
            {
                Elevation = true,
                CloseAfterLogin = true
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Vm.Password = (sender as PasswordBox)?.SecurePassword;
        }
    }
}
