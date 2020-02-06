/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Security;
using System.Threading.Tasks;
using TheXDS.MCART.Security.Password;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.ViewModels
{
    /// <summary>
    /// ViewModel que controla el inicio de sesión.
    /// </summary>
    public class LoginViewModel : PageViewModel
    {
        private string _user;
        private SecureString _password;
        private bool _loginSucceeded;
        private string _errorMessage;
               
        /// <summary>
        /// Obtiene o establece un valor que indica si se llamará al método
        /// de cerrado asociado a este <see cref="LoginViewModel"/> luego
        /// de un inicio de sesión válido.
        /// </summary>
        public bool CloseAfterLogin { get; set; }

        /// <summary>
        /// Obtiene o establece el usuario que se autenticará.
        /// </summary>
        public string User
        {
            get => _user;
            set
            {
                Change(ref _user, value);
                ErrorMessage = null;
            }
        }
  
        /// <summary>
        /// Obtiene o establece la contraseña a utilizar para autenticarse.
        /// </summary>
        public SecureString Password
        {
            get => _password;
            set
            {
                Change(ref _password, value);
                ErrorMessage = null;
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si la operación de inicio de sesión
        /// fue exitosa.
        /// </summary>
        public bool Success
        {
            get => _loginSucceeded;
            private set => Change(ref _loginSucceeded, value);
        }
       
        /// <summary>
        /// Obtiene un valor que determina si ha ocurrido un error de
        /// inicio de sesión.
        /// </summary>
        public bool Failed => ErrorMessage != null;
  
        /// <summary>
        /// Obtiene un mensaje de error de inicio de sesión.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (Change(ref _errorMessage, value))
                    OnPropertyChanged(nameof(Failed));
            }
        }
  
        /// <summary>
        /// Obtiene un comando que al ejecutarse intenta iniciar sesión.
        /// </summary>
        public SimpleCommand LoginCommand { get; }
 
        /// <summary>
        /// Se produce cuando se ha iniciado sesión correctamente.
        /// </summary>
        public event EventHandler<LoginSuccessEventArgs> LoginSuccess;

        /// <summary>
        /// Se produce cuando el inicio de sesión ha fallado.
        /// </summary>
        public event EventHandler LoginFailed;

        /// <summary>
        /// Se produce cuando se cancela el intento de inicio de sesión.
        /// </summary>
        public event EventHandler LoginCancel;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="LoginViewModel"/>.
        /// </summary>
        public LoginViewModel(ICloseable host) : base(host, true)
        {
            LoginCommand = new SimpleCommand(OnLogin);
            Title = "Iniciar sesión";
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="LoginViewModel"/> a partir de una credencial con los
        /// valores de usuario y contraseña a utilizar.
        /// </summary>
        /// <param name="host">Host visual que hospeda a esta página.</param>
        /// <param name="fromCredential">
        /// Credencial inicial a mostrar.
        /// </param>
        public LoginViewModel(ICloseable host, ICredential fromCredential) : this(host)
        {
            User = fromCredential.Username;
            Password = fromCredential.Password;
            if (Password != null) OnLogin();
        }

        /// <summary>
        /// Cierra esta página.
        /// </summary>
        public override sealed void Close()
        {
            LoginCancel?.Invoke(this, EventArgs.Empty);
            base.Close();
        }

        private async void OnLogin()
        {
            //TODO: Fixear un bug feo de login con todo en null
            if (User.IsEmpty())
            {
                ErrorMessage = "Usuario requerido.";
                return;
            }
            if (Password is null || Password.Length == 0)
            {
                ErrorMessage = "Contraseña requerida.";
                return;
            }
            await PerformAsync(PerformLogin, LoginCommand, CloseCommand);
            if (CloseAfterLogin && Success) base.Close();
        }
        private async Task PerformLogin()
        {
            var p = Password.Copy();
            var t = Elevation 
                ? Proteus.LogonService!.Login(User, Password) 
                : Proteus.Login(User, Password);
            Proteus.CommonReporter?.UpdateStatus("Iniciando sesión...");
            var r = await t;
            Success = r;
            ErrorMessage = r;
            if (r)
            {
                if (r.Logon.ScheduledPasswordChange)
                {
                    if (MCART.Dialogs.PasswordDialog.ConfirmPassword(out var udata))
                    {
                        await UserService.ChangePassword(r.Logon, udata.Password);
                    }
                }
                LoginSuccess?.Invoke(this, new LoginSuccessEventArgs(new Component.Credential(r.Logon.StringId, p))); 
            }
            else LoginFailed?.Invoke(this, EventArgs.Empty);
            Proteus.CommonReporter?.Done();
        }

        private bool _elevation;

        /// <summary>
        /// Obtiene o establece el valor Elevation.
        /// </summary>
        /// <value>El valor de Elevation.</value>
        public bool Elevation
        {
            get => _elevation;
            set => Change(ref _elevation, value);
        }


        public class LoginSuccessEventArgs : EventArgs
        {
            public ICredential Credential { get; }
            public LoginSuccessEventArgs(ICredential credential)
            {
                Credential = credential;
            }
        }
    }
}