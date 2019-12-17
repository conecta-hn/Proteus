/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TheXDS.MCART.PluginSupport;
using System.Windows.Threading;
using TheXDS.MCART.Types.Extensions;
using System.Reflection;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Component;
using System.Diagnostics;

namespace TheXDS.Proteus
{
    public class ExceptionDumper : Tool
    {
        private const string _totalFail = "Error cerrando la aplicación. Para prevenir daños a la información o al equipo, detenga la ejecución de este programa inmediatamente.";
        static ExceptionDumper()
        {
            Hook();
        }

        private static void Hook()
        {
            if (_enabled) return;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Application.Current.DispatcherUnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            _enabled = true;
        }
        private static void UnHook()
        {
            if (!_enabled) return;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            Application.Current.Dispatcher.UnhandledException -= Dispatcher_UnhandledException;
            Application.Current.DispatcherUnhandledException -= Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            _enabled = false;
        }

        private static bool _dumping;
        private static bool _enabled;
        private static bool _shutdown = true;

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                ProteusLib.MessageTarget?.Critical(e.Exception);
                e.SetObserved();
                if (!_dumping) Dump(e.Exception);
                if (_shutdown)
                {
                    Application.Current.Dispatcher.Invoke(() => Application.Current?.Shutdown(e.Exception?.HResult ?? -1));
                }
                else
                {
                    if (Debugger.Launch()) Debugger.Break();
                }
            }
            catch (Exception ex)
            {
                ProteusLib.MessageTarget?.Critical(_totalFail);
                Environment.FailFast(_totalFail, ex);
            }
        }
        private static void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                ProteusLib.MessageTarget?.Critical(e.Exception);
                e.Handled = true;
                if (!_dumping) Dump(e.Exception);
                if (_shutdown)
                {
                    Application.Current.Dispatcher.Invoke(() => Application.Current?.Shutdown(e.Exception?.HResult ?? -1));
                }
                else
                {
                    if (Debugger.Launch()) Debugger.Break();
                }
            }
            catch (Exception ex)
            {
                ProteusLib.MessageTarget?.Critical(_totalFail);
                Environment.FailFast(_totalFail, ex);
            }
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                ProteusLib.MessageTarget?.Critical(e.ExceptionObject as Exception);
                if (!_dumping) Dump(e.ExceptionObject as Exception);
                if (_shutdown)
                {
                    Application.Current.Dispatcher.Invoke(() => Application.Current?.Shutdown((e.ExceptionObject as Exception)?.HResult ?? -1));
                }
                else
                {
                    if (Debugger.Launch()) Debugger.Break();
                }
            }
            catch (Exception ex)
            {
                ProteusLib.MessageTarget?.Critical(_totalFail);
                Environment.FailFast(_totalFail, ex);
            }
        }

        [InteractionItem, Name("💣"), Description("Genera una excepción de manera intencional.")]
        public void IntentionallyCrash(object sender, EventArgs e)
        {
            /* NOTA:
             * Este bloque genera intencionalmente una excepción de forma 
             * aleatoria. Obviamente, si el depurador se detiene aquí, se debe
             * hacer caso omiso, y simplemente dejar que la ejecución continúe
             * normalmente para que este plugin continúe con su propósito.
             */
            throw TheXDS.MCART.Objects.GetTypes<Exception>(true).Pick().New<Exception>();
        }

        [InteractionItem, Name("💣😉"), Description("Simula una excepción en el sistema y muestra una ventana de error crítico.")]
        public void IntentionallyPick(object sender, EventArgs e)
        {
            Exception ex;
            try
            {
                ex = TheXDS.MCART.Objects.GetTypes<Exception>(true).Pick().New<Exception>();
            }
            catch (Exception ee)
            {
                ex = ee;
            }
            ProteusLib.MessageTarget?.Critical(ex);
        }

        [InteractionItem, Name("🕷"), Description("Activa/desactiva la captura de excepciones por este plugin.")]
        public void ToggleCatch(object sender, EventArgs e)
        {
            if (_enabled) UnHook();
            else Hook();
            var m = $"Captura de excepciones {(_enabled ? "activada" : "desactivada")}.";
            ProteusLib.MessageTarget?.Info(m);
            ProteusLib.AlertTarget?.Alert(m, _enabled
                ? "Las excepciones no controladas serán capturadas y volcadas a un archivo de texto en el escritorio."
                : "Las excepciones no controladas no serán capturadas.");
        }

        [InteractionItem, Name("🕷→❌"), Description("Activa/desactiva el cierre forzoso de la aplicación al producirse un error crítico.")]
        public void ToggleShutdown(object sender, EventArgs e)
        {
            _shutdown = !_shutdown;
            var m = $"Cierre forzoso {(_shutdown ? "activado" : "desactivado")}.";
            ProteusLib.MessageTarget?.Info(m);
            ProteusLib.AlertTarget?.Alert(m, _shutdown
                ? "Las excepciones críticas causarán el cierre de la aplicación."
                : "Las excepciones críticas no causarán el cierre de la aplicación.");
        }

        private static void Dump(Exception ex)
        {
            _dumping = true;
            try
            {
                using var j = new System.IO.StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Proteus-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}-failure.txt");
                Dump(j, ex);

                PrintInfo(j, new ApplicationInfo(Application.Current),"Aplicación cliente");
                if (!(ex.TargetSite is null)) PrintInfo(j, new AssemblyInfo(ex.TargetSite.DeclaringType.Assembly), "Ensamblado con errores");
                j.WriteLine("Componentes (en orden de carga):");
                foreach (var k in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        PrintInfo(j, new AssemblyInfo(k));
                    }
                    catch { }
                }
            }
            catch { /* Everything failed. */ }
        }
        private static void PrintInfo(System.IO.StreamWriter j, IExposeInfo nfo) => PrintInfo(j, nfo, null);
        private static void PrintInfo(System.IO.StreamWriter j, IExposeInfo nfo, string name)
        {
            j.WriteLine($"{name.OrNull("{0}: ")}{nfo.Name} {nfo.InformationalVersion}");
        }

        private static void Dump(System.IO.StreamWriter j, Exception ex)
        {
            const int TextWidth = 80;
            if (ex is null) return;

            j.WriteLine($"{ex.GetType().Name} en {ex.Source} (0x{ex.HResult.ToString("X").PadLeft(8, '0')})");
            j.WriteLine(new string('-', TextWidth));
            foreach (var k in Breakup(ex.Message)) j.WriteLine(k.TrimEnd(' '), TextWidth);
            j.WriteLine(new string('-', TextWidth));
            j.WriteLine(ex.StackTrace);
            j.WriteLine(new string('=', TextWidth));

            switch (ex)
            {
                case AggregateException aggregateEx:
                    foreach (var k in aggregateEx.InnerExceptions) Dump(j, k);
                    break;
                case ReflectionTypeLoadException typeLoadEx:
                    foreach (var k in typeLoadEx.LoaderExceptions) Dump(j, k);
                    break;
            }
            Dump(j, ex?.InnerException);
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
    }
}