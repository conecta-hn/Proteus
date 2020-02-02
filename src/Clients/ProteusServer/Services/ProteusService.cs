/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Daemons;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TheXDS.MCART;
using TheXDS.MCART.Events;
using TheXDS.MCART.Networking.Server;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types.Extensions;
using static TheXDS.MCART.Types.Extensions.EnumerableExtensions;
using TheXDS.MCART.Exceptions;

namespace TheXDS.Proteus.Services
{
    internal class ProteusService : ServiceBase, IMessageTarget
    {
        /// <summary>
        /// Arranca manualmente el servicio.
        /// </summary>
        public void StartService() => OnStart(Array.Empty<string>());

        internal readonly HashSet<IProteusProtocolPlugin> _plugins = new HashSet<IProteusProtocolPlugin>();
        public readonly HashSet<Server> RunningServers = new HashSet<Server>();
        private readonly HashSet<IDaemon> _daemons = new HashSet<IDaemon>();
        private readonly System.Timers.Timer _timer = new System.Timers.Timer() { AutoReset = false };


        /// <inheritdoc />
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="T:TheXDS.Proteus.Infrastructure.ProteusService" />.
        /// </summary>
        public ProteusService()
        {
            ServiceName = "ProteusService";
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private static string DumpSettings()
        {
            var sb = new StringBuilder();
            foreach (var j in typeof(Settings).GetProperties())
            {
                try
                {
                    var v = j.GetValue(Settings.Default)?.ToString();
                    sb.AppendLine($"  - {j.NameOf()}: {v ?? "<null>"}");
                }
                catch { }
            }
            return sb.ToString();
        }

        /// <inheritdoc />
        /// <summary>
        /// Cuando se implementa en una clase derivada, se ejecuta cuando
        /// se envía un comando de inicio al servicio mediante el
        /// Administrador de Control de servicios (SCM) o cuando se inicia
        /// el sistema operativo (para un servicio que se inicia
        /// automáticamente).
        /// Especifica las acciones que deben realizarse cuando se inicia
        /// el servicio.
        /// </summary>
        /// <param name="args">Datos pasados por el comando de inicio.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                Proteus.MessageTarget?.Info($"Configuración de inicio:\n{DumpSettings()}  - Directorio de arranque: {Environment.CurrentDirectory}");
                Proteus.MessageTarget?.Show("Cargando Servicios de datos...");

                if (Proteus.Services.Any())
                    Proteus.MessageTarget?.Info($"Proteus ha encontrado {Proteus.Services!.Count} servicios de datos:\n  - {string.Join("\n  - ", Proteus.Services.Select(p => p.Name))}");
                else
                    Proteus.MessageTarget?.Warning("Proteus no ha encontrado ningún servicio de datos.");

                Scan();

                lock (RunningServers)
                lock (_plugins)
                {
                    foreach (var j in BuildServers())
                    {
                        try
                        {
                            j.ServerStarted += Srv_ServerStarted;
                            j.ServerStopped += Srv_ServerStopped;
                            j.ClientConnected += Srv_ClientConnected;
                            j.ClientRejected += Srv_ClientRejected;
                            j.Start();
                            RunningServers.Add(j);
                        }
                        catch (Exception ex)
                        {
                            Proteus.MessageTarget?.Critical(ex);
                        }
                    }
                }

                if (_daemons.Any())
                {
                    Proteus.MessageTarget?.Info($"El servicio ha encontrado {_daemons.Count} Daemons:\n  - {string.Join("\n  - ", _daemons.Select(DescribeDaemon))}");
                    _timer.Elapsed += RunDaemons;
                    SetupTimer();
                }
                else
                {
                    Proteus.MessageTarget?.Info("No se han encontrado Daemons para ejecutar.");
                }

                Proteus.MessageTarget?.Show($"Servicio iniciado correctamente. Servidores en ejecución: {RunningServers.Count}");
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Critical(ex);
                if (Settings.Default.CatchStartupError)
                    Proteus.MessageTarget?.Error("El servicio ha encontrado un error. Debe reiniciarlo manualmente.");
                else throw;
            }
        }
        public void RunDaemons()
        {
            RunDaemons(null, null);
        }

        public void RunDaemons(object? sender, ElapsedEventArgs? e)
        {
            Proteus.MessageTarget?.Info("Ejecución programada de daemons");
            foreach(var j in _daemons
                .Where(p => p.Schedule == _schedule)
                .Where(p => p.CanRun))
            {
                j.Run();
            }
            SetupTimer();
        }

        private DateTime NextDaemonSchedule(out DateTime reference)
        {
            reference = DateTime.Now;
            return reference + TimeSpan.FromMinutes(15.0 - ((reference.Minute % 15) + (reference.Second / 60.0) + (reference.Millisecond / 60000.0)));
        }

        private void SetupTimer()
        {
            var t = NextDaemonSchedule(out var r);
            _schedule = (byte)(t.TimeOfDay.TotalMinutes / 15);
            var m = (t - r).TotalMilliseconds;
            _timer.Interval = m > 1000 ? m : 900000-m;
            _timer.Start();
            Proteus.MessageTarget?.Info($"Próxima ejecución de Daemons programada para {t}");
        }
        private byte _schedule;

        private static string DescribeDaemon(IDaemon p)
        {
            var sb = new StringBuilder();
            sb.AppendLine((p as IPlugin)?.Name ?? p.GetType().NameOf());
            sb.AppendLine($"    Programación de inicio: {p.ScheduleTime}");
            return sb.ToString();
        }

        protected override void OnStop()
        {
            lock (RunningServers) foreach (var j in RunningServers) j.Stop();
            Proteus.MessageTarget?.Show("Servicio detenido correctamente.");
        }

        private void Srv_ClientRejected(object? sender, ValueEventArgs<Client?> e)
        {
            var srv = sender as Server ?? throw new TamperException();
            Proteus.MessageTarget?.Warning($"El cliente {e.Value?.EndPoint?.ToString() ?? "desconocido"} ha sido rechazado por '{srv.Protocol.GetType().NameOf()}'.");
        }
        private void Srv_ClientConnected(object? sender, ValueEventArgs<Client> e)
        {
            var srv = sender as Server ?? throw new TamperException();
            Proteus.MessageTarget?.Show($"'{srv.Protocol.GetType().NameOf()}': Conexión entrante desde {e.Value.EndPoint}");
        }
        private void Srv_ServerStopped(object? sender, ValueEventArgs<DateTime> e)
        {
            var srv = sender as Server ?? throw new TamperException();
            Proteus.MessageTarget?.Warning($"'{srv.Protocol.GetType().NameOf()}' se ha detenido.");
        }
        private void Srv_ServerStarted(object? sender, ValueEventArgs<DateTime> e)
        {
            var srv = sender as Server ?? throw new TamperException();
            (srv.Protocol as IAnnounceAvailability)?.Announce();
            Proteus.MessageTarget?.Show($"'{srv.Protocol.GetType().NameOf()}' iniciado correctamente. Escuchando conexiones en {srv.ListeningEndPoint}");
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Proteus.MessageTarget?.Critical(e.Exception ?? new AggregateException(new TamperException()));
            e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Proteus.MessageTarget?.Critical(e.ExceptionObject as Exception ?? new TamperException());
        }

        #region Métodos auxiliares

        private void Scan(bool clear = true)
        {
            Proteus.MessageTarget?.Show("Cargando servicios de red...");
            LockAndLoad(_plugins, clear);
            Proteus.MessageTarget?.Show("Cargando daemons...");
            LockAndLoad(_daemons, clear);
        }

        private void LockAndLoad<T>(ICollection<T> plugins, bool clear) where T : class
        {
            plugins.Locked(p =>
            {
                if (clear) p.Clear();
                SafelyLoad(p, false);
            });
        }

        private void SafelyLoad<T>(ICollection<T> plugins, bool @lock = true) where T : class
        {
            try
            {
                if (@lock) plugins.Locked(Load);
                else Load(plugins);
            }
            catch (Exception ex) { Proteus.MessageTarget?.Critical(ex); }
        }
        
        private void Load<T>(ICollection<T> plugins) where T : class
        {
            var loader = new PluginLoader();
            foreach (var j in Objects.FindAllObjects<T>().Concat(loader.LoadEverything<T>(Settings.Default.ServerPluginsDir)))
            {
                if (plugins.Any(p => p.GetType() == j.GetType())) continue;
                plugins.Add(j);
                Proteus.MessageTarget?.Show($"Se ha cargado {(j as IPlugin)?.Description ?? j.GetType().NameOf()}");
            }
        }


        private IEnumerable<Server> BuildServers()
        {
            return Objects.FindAllObjects<IProteusProtocol>().Select(p => p.BuildServer())
                .Concat(_plugins.Select(p => p.BuildServer()));
        }

        #endregion

        #region IMessageTarget

        public void Show(string message)
        {
            EventLog.WriteEntry(message);
        }
        public void Show(string title, string message)
        {
            EventLog.WriteEntry($"{title}\n{message}");
        }
        public void Info(string message)
        {
            EventLog.WriteEntry(message, EventLogEntryType.Information);
        }
        public void Warning(string message)
        {
            EventLog.WriteEntry(message, EventLogEntryType.Warning);
        }
        public void Stop(string message)
        {
            EventLog.WriteEntry($"Operación detenida: {message}", EventLogEntryType.Warning);
        }
        public void Error(string message)
        {
            EventLog.WriteEntry(message, EventLogEntryType.Error);
        }
        public void Critical(string message)
        {
            EventLog.WriteEntry($"Error crítico: {message}", EventLogEntryType.Error);
        }
        public void Critical(Exception ex)
        {
            EventLog.WriteEntry($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}", EventLogEntryType.Error);

            if (!(ex.InnerException is null)) Critical(ex.InnerException);
            switch (ex)
            {
                case AggregateException aex:
                    foreach (var j in aex.InnerExceptions) Critical(j);
                    break;
                case ReflectionTypeLoadException rex:
                    foreach (var j in rex.LoaderExceptions?.NotNull() ?? Array.Empty<Exception>()) Critical(j);
                    break;
            }
        }

        #endregion
    }
}