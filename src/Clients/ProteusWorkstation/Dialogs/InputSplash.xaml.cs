/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using SourceChord.FluentWPF;
using System.Windows;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using System.ComponentModel;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Proteus.Dialogs
{
    /// <summary>
    /// Lógica de interacción para InputSplash.xaml
    /// </summary>
    public partial class InputSplash : AcrylicWindow, ICloseable
    {
        private InputSplash()
        {
            InitializeComponent();
        }

        public static bool GetNew<T>(string prompt, out T value)
        {
            value = default!;
            return Get(prompt, ref value);
        }

        public static bool Get<T>(string prompt, ref T value)
        {
            var descr = new InputSplashDescription
            {
                Label = prompt,
                Property = typeof(InputSplashViewModel<T>).GetProperty(nameof(InputSplashViewModel<T>.InputValue))!
            };
            var dialog = new InputSplash();
            var vm = new InputSplashViewModel<T>(dialog, descr)
            {
                InputValue = value
            };           
            dialog.DataContext = vm;
            dialog.ShowDialog();
            value = vm.InputValue;
            return !vm.Cancel;
        }
    }

    public class InputSplashViewModel<T> : NotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ICloseable _host;
        private T _inputValue;

        /// <summary>
        /// Obtiene o establece el valor Title.
        /// </summary>
        /// <value>El valor de Title.</value>
        public string Prompt { get; }

        /// <summary>
        /// Obtiene un valor que indica si la obtención de un valor ha sido cancelada.
        /// </summary>
        public bool Cancel { get; private set; }

        /// <summary>
        /// Obtiene el control de edición a utilizar para obtener el valor
        /// introducido por el usuario.
        /// </summary>
        public FrameworkElement InputControl { get; }

        /// <summary>
        /// Obtiene el comando relacionado a la acción Close.
        /// </summary>
        /// <returns>El comando Close.</returns>
        public SimpleCommand CloseCommand { get; }

        /// <summary>
        /// Obtiene el comando relacionado a la acción Go.
        /// </summary>
        /// <returns>El comando Go.</returns>
        public SimpleCommand GoCommand { get; }


        public InputSplashViewModel(ICloseable host, IPropertyDescription description)
        {
            _host = host;
            CloseCommand = new SimpleCommand(OnClose);
            GoCommand = new SimpleCommand(OnGo);
            InputControl = PropertyMapper.GetMapping(description)?.Control;
            Prompt = description.Label;
        }

        private void OnClose()
        {
            Cancel = true;
            InputValue = default!;
            _host.Close();
        }

        private void OnGo()
        {
            _host.Close();
        }

        public T InputValue
        {
            get => _inputValue; 
            set => Change(ref _inputValue, value);
        }
    }
}