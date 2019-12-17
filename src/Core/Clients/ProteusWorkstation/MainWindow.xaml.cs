/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace TheXDS.Proteus
{
    /// <summary>
    ///     Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IPageVisualHost
    {
        private MainWindowViewModel Vm => Dispatcher.Invoke(()=> DataContext as MainWindowViewModel);

        /// <summary>
        ///     Obtiene una referencia al host de ventanas de esta instancia.
        /// </summary>
        public IPageHost PageHost { get; set; } = null;

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DataContext = new MainWindowViewModel(this);
            Closing += MainWindow_Closing;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_forced)
            {
                if (Settings.Default.ConfirmExit)
                    e.Cancel = !MessageSplash.Ask("Salir", "¿Está seguro que desea salir?");
                if (e.Cancel) return;
            }
            e.Cancel = false;
            PageHost?.Pages.Clear();
            Proteus.Logout(false);
        }

        private void MainWindow_Closed(object sender, EventArgs e) => Application.Current.Shutdown();

        /// <summary>
        ///     Realiza tareas de inicialización una vez abierta la ventana.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(_ => OnLoaded()));
        }

        private async void OnLoaded()
        {
            try
            {
                await Vm.LaunchAsync();
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Stop($@"Ha ocurrido un problema iniciando {App.Info.Name}.

Asegúrese de que:

    · La aplicación incluya todos los componentes.
    · La aplicación haya sido configurada correctamente.
    · Se esté usando un usuario de dominio en el equipo si la conexión es a un servidor de dominio.
    · Esté conectado a la red.

Información adicional: {ex.Message}
");
                //Proteus.MessageTarget?.Critical(ex);
                App.UiInvoke(ForceClose);
            }
        }

        private static void Dump(StringBuilder j, Exception ex)
        {
            const int TextWidth = 80;
            if (ex is null) return;

            j.AppendLine($"{ex.GetType().Name} en {ex.Source} (0x{ex.HResult.ToString("X").PadLeft(8, '0')})");
            j.AppendLine(new string('-', TextWidth));
            foreach (var k in Breakup(ex.Message)) j.AppendLine(k.TrimEnd(' '));
            j.AppendLine(new string('-', TextWidth));
            j.AppendLine(ex.StackTrace);
            j.AppendLine(new string('=', TextWidth));

            foreach (var k in ex.GetType().GetProperties())
            {
                if (typeof(IEnumerable<Exception>).IsAssignableFrom(k.PropertyType))
                {
                    foreach (var l in (IEnumerable<Exception>) k.GetValue(ex))
                    {
                        Dump(j, l);
                    }
                }
                else if (typeof(Exception).IsAssignableFrom(k.PropertyType))
                {
                    Dump(j, k.GetValue(ex) as Exception);

                }
            }
        }

        private static string[] Breakup(string str, int width = 80)
        {
            var l = new List<string>
            {
                string.Empty
            };

            foreach (var j in str.Split(' '))
            {
                if (string.IsNullOrEmpty(j)) continue;
                if (l.Last().Length + j.Length > width)
                {
                    l.Add($"{j} ");
                }
                else
                {
                    l[l.Count - 1] += $"{j} ";
                }
            }
            return l.ToArray();
        }

        /// <summary>
        ///     Activa una tab abierta en la ventana.
        /// </summary>
        /// <param name="page"></param>
        public void Activate(IPage page)
        {
            TabHost.SelectedItem = page;
        }

        void IPage.Activate()
        {
            BringIntoView();
        }

        internal void ForceClose()
        {
            _forced = true;
            Close();
        }
        private bool _forced = false;
    } 
}