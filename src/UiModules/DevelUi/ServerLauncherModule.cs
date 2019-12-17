/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Plugins;
using System;
using System.Linq;
using System.Diagnostics;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus
{
    [Name("Server Launcher Module")]
    public class ServerLauncherModule : UiModule
    {
        static Process _srvProc;
        /// <summary>
        ///     Inicializa la clase <see cref="ServerLauncherModule"/>
        /// </summary>
        static ServerLauncherModule()
        {
            if (Environment.GetCommandLineArgs().Any(p => p == "-LaunchServer"))
            {
                LaunchServer();
                AppDomain.CurrentDomain.ProcessExit += KillServer;
            }
        }

        private static void KillServer(object sender, EventArgs e)
        {
            if (_srvProc is null) return;
            try
            {
                _srvProc.Kill();
            }
            finally
            {
                _srvProc.Dispose();
                _srvProc = null;
            }
        }
        private static void LaunchServer()
        {
            if (!(_srvProc is null)) return;
            try
            {
                _srvProc = Process.Start(@"..\..\Server\net47\ProteusServer.exe",@"--chdir:..\..\Server\net47");
            }
            catch(Exception ex)
            {
                ProteusLib.MessageTarget?.Critical(ex);
            }
            finally
            {
                _srvProc.Dispose();
                _srvProc = null;
            }
        }
    }
}