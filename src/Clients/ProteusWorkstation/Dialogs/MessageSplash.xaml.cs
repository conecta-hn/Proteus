/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using SourceChord.FluentWPF;
using System;
using System.Windows;
using System.Windows.Media;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageSplash.xaml
    /// </summary>
    public partial class MessageSplash : AcrylicWindow, ICloseable
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="MessageSplash"/>.
        /// </summary>
        public MessageSplash()
        {
            InitializeComponent();
            Loaded += MessageSplash_Loaded;
        }

        private void MessageSplash_Loaded(object sender, RoutedEventArgs e)
        {
            BtnClose.Focus();
        }

        /// <summary>
        /// Muestra una mensaje por medio de una ventana
        /// <see cref="MessageSplash"/>.
        /// </summary>
        /// <param name="title">Título del mensaje.</param>
        /// <param name="message">Mensaje a mostrar.</param>
        /// <param name="type">
        /// Tipo de mensaje. Afecta la apariencia de la ventana generada.
        /// </param>
        public static void Show(string title, string message, MessageType type)
        {
            App.UiInvoke(() => {
                var m = new MessageSplash();
                m.DataContext = new MessageSplashViewModel(m, title, message, type);
                m.ShowDialog();
            });
        }

        /// <summary>
        /// Realiza una pregunta al usuario.
        /// </summary>
        /// <param name="title">Título del mensaje.</param>
        /// <param name="message">Contenido de la pregunta.</param>
        /// <returns>
        /// <see langword="true"/> si el usuario ha dicho que sí al cuadro
        /// de diálogo, <see langword="false"/> en caso contrario.
        /// </returns>
        public static bool Ask(string title, string message)
        {
            return Application.Current.Dispatcher.Invoke(() => {
                var m = new MessageSplash();
                m.DataContext = new MessageSplashViewModel(m, title, message, MessageType.Question);
                m.ShowDialog();
                return ((MessageSplashViewModel)m.DataContext).Result;
            });
        }

        /// <summary>
        /// Realiza una pregunta al usuario.
        /// </summary>
        /// <param name="title">Título del mensaje.</param>
        /// <returns>
        /// <see langword="true"/> si el usuario ha dicho que sí al cuadro
        /// de diálogo, <see langword="false"/> en caso contrario.
        /// </returns>
        public static bool Ask(string title)
        {
            return Ask(title, "¿Está seguro que desea realizar esta operación?");
        }
    }

    /// <summary>
    /// Define el comportamiento de una instancia de la clase
    /// <see cref="MessageSplash"/>.
    /// </summary>
    public class MessageSplashViewModel: ViewModelBase
    {
        private readonly ICloseable _host;
        private bool _result;

        /// <summary>
        /// Obtiene o establece el valor Title.
        /// </summary>
        /// <value>El valor de Title.</value>
        public string Title { get; }

        /// <summary>
        /// Obtiene o establece el valor Message.
        /// </summary>
        /// <value>El valor de Message.</value>
        public string Message { get; }

        /// <summary>
        /// Obtiene o establece el valor Type.
        /// </summary>
        /// <value>El valor de Type.</value>
        public MessageType Type { get; }

        /// <summary>
        /// Obtiene el color de fondo a utilizar para la ventana.
        /// </summary>
        public Color Background
        {
            get
            {
                return Type switch
                {
                    MessageType.Question => SystemTheme.AppTheme == ApplicationTheme.Light
                           ? Colors.PaleGreen
                           : Colors.DarkGreen,
                    MessageType.Warning => SystemTheme.AppTheme == ApplicationTheme.Light
                           ? Colors.LightGoldenrodYellow
                           : Colors.DarkGoldenrod,
                    MessageType.Error => SystemTheme.AppTheme == ApplicationTheme.Light
                            ? Colors.Pink
                            : Colors.PaleVioletRed,
                    MessageType.Critical => SystemTheme.AppTheme == ApplicationTheme.Light
                            ? Colors.IndianRed
                            : Colors.DarkRed,
                    _ => SystemTheme.AppTheme == ApplicationTheme.Light
                            ? Colors.LightGray
                            : Colors.DarkGray,
                };
            }
        }

        /// <summary>
        /// Obtiene el <see cref="Brush"/> a utilizar para dibujar el ícono
        /// a la izquierda del mensaje.
        /// </summary>
        public Brush IconBrush
        {
            get
            {
                switch (Type)
                {
                    case MessageType.Info:
                        return Brushes.CornflowerBlue;
                    case MessageType.Question:
                        return Brushes.SpringGreen;
                    case MessageType.Warning:
                        return Brushes.Gold;
                    case MessageType.Stop:
                        return Brushes.Red;
                    case MessageType.Error:
                    case MessageType.Critical:
                        return SystemTheme.AppTheme == ApplicationTheme.Light
                            ? Brushes.Black
                            : Brushes.White;
                    default:
                        return AccentColors.ImmersiveSystemAccentBrush;
                }
            }
        }

        /// <summary>
        /// Obtiene el ícono a mostrar a la izquierda del mensaje.
        /// </summary>
        public string? Icon
        {
            get
            {
                return Type switch
                {
                    MessageType.Question => "❓",
                    MessageType.Info => "ℹ",
                    MessageType.Warning => "⚠",
                    MessageType.Stop => "🛑",
                    MessageType.Error => "❌",
                    MessageType.Critical => "💣",
                    _ => null,
                };
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="MessageSplashViewModel"/>.
        /// </summary>
        /// <param name="host">
        /// Ventana host de este ViewModel.
        /// </param>
        /// <param name="title">
        /// Título a mostrar en la ventana de mensaje.
        /// </param>
        /// <param name="message">
        /// Mensaje a desplegar.
        /// </param>
        /// <param name="type">
        /// Tipo de mensaje. Cambia la apariencia de la ventana generada.
        /// </param>
        public MessageSplashViewModel(ICloseable host, string title, string message, MessageType type)
        {
            _host = host;
            CloseCommand = new SimpleCommand(OnClose);
            OkCommand = new SimpleCommand(OnOk);
            Title = title;
            Message = message;
            Type = type;
        }

        /// <summary>
        /// Obtiene el comando relacionado a la acción Close.
        /// </summary>
        /// <returns>El comando Close.</returns>
        public SimpleCommand CloseCommand { get; }

        private void OnClose()
        {
            _host.Close();
        }

        private void OnOk()
        {
            Result = true;
            OnClose();
        }

        /// <summary>
        /// Obtiene el comando realcionado a la acción de aceptación de una pregunta.
        /// </summary>
        public SimpleCommand OkCommand { get; }

        /// <summary>
        /// Obtiene un valor de visibilidad para las preguntas.
        /// </summary>
        public Visibility QuestionVis => Type == MessageType.Question ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Obtiene la etiqueta a mostrar en el botón para cerrar la ventana.
        /// </summary>
        public string CloseLabel => Type == MessageType.Question ? "No" : "Cerrar";

        /// <summary>
        /// Obtiene el valor de resultado del cuadro de diálogo.
        /// </summary>
        public bool Result
        {
            get => _result;
            set => Change(ref _result,value);
        }
    }

    /// <summary>
    /// <see cref="IMessageTarget"/> para mostrar mensajes por medio de una
    /// instancia de la clase <see cref="MessageSplash"/>.
    /// </summary>
    public class MessageSplashTarget : IInteractiveMessageTarget
    {
        /// <summary>
        /// Realiza una pregunta al usuario.
        /// </summary>
        /// <param name="title">Título del mensaje.</param>
        /// <param name="message">Contenido de la pregunta.</param>
        /// <returns>
        /// <see langword="true"/> si el usuario ha dicho que sí al cuadro
        /// de diálogo, <see langword="false"/> en caso contrario.
        /// </returns>
        public bool Ask(string title, string message) => MessageSplash.Ask(title, message);

        /// <summary>
        /// Realiza una pregunta al usuario.
        /// </summary>
        /// <param name="title">Título del mensaje.</param>
        /// <returns>
        /// <see langword="true"/> si el usuario ha dicho que sí al cuadro
        /// de diálogo, <see langword="false"/> en caso contrario.
        /// </returns>
        public bool Ask(string title) => MessageSplash.Ask(title);

        /// <summary>
        /// Notifica de un error crítico.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        public void Critical(string message)
        {
            MessageSplash.Show("Error crítico", message, MessageType.Critical);
        }

        /// <summary>
        /// Notifica de un error crítico.
        /// </summary>
        /// <param name="ex">
        /// Excepción producida a notificar.
        /// </param>
        public void Critical(Exception ex)
        {
            MessageSplash.Show("Error crítico", ex.Message, MessageType.Critical);
        }
        
        /// <summary>
        /// Notifica de un mensaje de error.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        public void Error(string message)
        {
            MessageSplash.Show("Error", message, MessageType.Error);
        }

        /// <summary>
        /// Notifica de un mensaje informativo.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        public void Info(string message)
        {
            MessageSplash.Show("Información", message, MessageType.Info);
        }

        /// <summary>
        /// Notifica de un mensaje simple.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        public void Show(string message)
        {
            MessageSplash.Show("Mensaje", message, MessageType.Message);
        }

        /// <summary>
        /// Notifica de un mensaje simple.
        /// </summary>
        /// <param name="title">
        /// Título del mensaje.
        /// </param>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        public void Show(string title, string message)
        {
            MessageSplash.Show(title, message, MessageType.Message);
        }

        /// <summary>
        /// Notifica de un mensaje producido por una operación que debe
        /// detenerse.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        public void Stop(string message)
        {
            MessageSplash.Show("Alto", message, MessageType.Stop);
        }

        /// <summary>
        /// Notifica de una advertencia.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        public void Warning(string message)
        {
            MessageSplash.Show("Advertencia", message, MessageType.Warning);
        }
    }

    /// <summary>
    /// Enumera el tipo de mensaje a mostrar.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Mensaje.
        /// </summary>
        Message,

        /// <summary>
        /// Mensaje informativo.
        /// </summary>
        Info,

        /// <summary>
        /// Pregunta.
        /// </summary>
        Question,

        /// <summary>
        /// Advertencia.
        /// </summary>
        Warning,

        /// <summary>
        /// Detención de operación.
        /// </summary>
        Stop,

        /// <summary>
        /// Error.
        /// </summary>
        Error,

        /// <summary>
        /// Crítico.
        /// </summary>
        Critical
    }
}