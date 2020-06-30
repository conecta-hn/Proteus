/*
Copyright © 2017-2020 César Andrés Morgan
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

        private UacSplash(IProteusCredential? credential)
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this, new Credential(credential?.Id))
            {
                Elevation = true,
                CloseAfterLogin = true
            };
            Vm.LoginFailed += Vm_LoginFailed;
        }

        private void Vm_LoginFailed(object? sender, System.EventArgs e)
        {
            TxtPassword.Clear();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Vm.Password = (sender as PasswordBox)?.SecurePassword;
        }
        public static bool Elevate()
        {
            var uac = new UacSplash();
            uac.ShowDialog();
            return uac.Vm.Success;
        }

        public static bool Elevate(ref IProteusUserCredential? credential)
        {
            var uac = new UacSplash(credential);
            uac.ShowDialog();
            if (uac.Vm.Success)
            {
                credential = uac.Vm.Result.Logon as IProteusUserCredential;
                return true;
            }
            return false;
        }
    }
}
