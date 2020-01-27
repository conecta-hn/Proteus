/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Pages.Base;
using TheXDS.Proteus.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace TheXDS.Proteus.Pages
{
    /// <summary>
    /// Lógica de interacción para LoginPage.xaml
    /// </summary>
    public partial class SimpleLoginPage : ILoginPage
    {
        private static bool _launched;
        /// <summary>
        /// ViewModel que gestiona a esta página de Login.
        /// </summary>
        public new LoginViewModel ViewModel => base.ViewModel as LoginViewModel;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="SimpleLoginPage"/>.
        /// </summary>
        public SimpleLoginPage()
        {
            InitializeComponent();
            if (Settings.Default.RememberLastLogin && !_launched)
            {
                _launched = true;
                base.ViewModel = new LoginViewModel(this, Credential.FromSettings())
                {
                    CloseAfterLogin = true
                };
            }
            else
            {
                base.ViewModel = new LoginViewModel(this)
                {
                    CloseAfterLogin = true
                };
            }
        }

        /// <summary>
        /// Cierra esta página.
        /// </summary>
        public override void Close()
        {
            if (ViewModel.Success) base.Close();
            else Application.Current?.Shutdown();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.Password = (sender as PasswordBox)?.SecurePassword;
        }
    }
}