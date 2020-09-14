/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using TheXDS.MCART.Networking.Legacy.Server;

namespace TheXDS.Proteus
{
    /// <summary>
    /// Clase principal de ProteusServer.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Enumera a todos los servidores corriendo en el Host.
        /// </summary>
        public static IEnumerable<IServer> Servers => _service.RunningServers;

        internal static readonly ProteusService _service = new ProteusService();

        /// <summary>
        /// Obtiene un servidor que ejecute el protocolo especificado.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo del protocolo en ejecución del servidor que se desea obtener.
        /// </typeparam>
        /// <returns>
        /// Un <see cref="Server"/> que esté ejecutando el protocolo especificado.
        /// </returns>
        public static T? ServerOf<T>() where T : class, IProtocol
        {
            return Servers.FirstOrDefault(s => s.Protocol.GetType() == typeof(T))?.Protocol as T;
        }

        /// <summary>
        /// Punto de entrada de la aplicación.
        /// </summary>
        /// <param name="args">
        /// Argumentos de línea de comandos.
        /// </param>
        public static async Task Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                Proteus.MessageTarget = _service;
                await Proteus.Init();
                ServiceBase.Run(new ServiceBase[] { _service });
            }
            else
            {
                var a = await KickStart.Run();
                var tgt = new ConsoleMessageTarget();
                Proteus.MessageTarget = tgt;
                Proteus.CommonReporter = tgt; 
                Proteus.MessageTarget.Show("----- Host iniciado en modo interactivo -----");

                _service.StartService();
                Console.CancelKeyPress += OnExit;
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Detiene el servicio por completo.
        /// </summary>
        public static void Exit()
        {
            Proteus.MessageTarget?.Info("Deteniendo servidor...");
            Environment.Exit(0);
        }

        private static void OnExit(object sender, ConsoleCancelEventArgs e)
        {
            Exit();
        }
    }
}