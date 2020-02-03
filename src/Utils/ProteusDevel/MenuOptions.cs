/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheXDS.MCART;
using TheXDS.MCART.Cmd;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Extensions;
using static TheXDS.MCART.Objects;

namespace TheXDS.Proteus.Devel
{
    public class ExitMenuOption : MenuOption
    {
        public override string Summary => "Salir";

        internal override void Run()
        {
            ProteusDevel._appActive = false;
        }
    }

    public class HookTerminalMenuOption : MenuOption
    {
        private readonly RemoteTerminalClient _termClient = new RemoteTerminalClient();
        public override string Summary => $"{(_termClient.IsAlive ? "Desconectar":"Adjuntar")} terminal remota";

        internal override void Run()
        {
            if (_termClient.IsAlive)
            {
                _termClient.CloseConnection();
                return;
            }
            var s = Proteus.Settings.NetworkServerAddress;
            Proteus.CommonReporter?.UpdateStatus($"Conectando a {s}");
            Proteus.CommonReporter?.Done(_termClient.Connect(s, 51201)
                ? "Conexión realizada exitosamente."
                : "No fue posible conectar la terminal remota.");
        }
    }

    public class AuditMenuOption : MenuOption
    {
        public override string Summary => "Ejecutar auditoría de permisos";

        internal override void Run()
        {
            var u = Proteus.LogonService.Get<User>(Input("Usuario a auditar (Intro=usuario actual)").OrNull() ?? Proteus.Session.Id);
            if (u is null)
            {
                Proteus.MessageTarget?.Stop("Usuario desconocido.");
                return;
            }
            Audit(u);
            Separator('=');
            ShallowAudit(u);
        }

        private static void Audit(IProteusHierachicalCredential credential)
        {
            Console.WriteLine($"Auditoría de permisos para {credential.Name}");

            foreach (var j in Proteus.Services)
            {
                Console.WriteLine($"\n{j}\n---------------------------");
                foreach (var k in j.Audit(credential))
                {
                    PrintAudit(k);
                }
            }
        }
        private static void PrintAudit(KeyValuePair<MethodInfo, bool?> k)
        {
            Console.Write($"{k.Key.NameOf()}: ");
            if (k.Value.HasValue)
            {
                if (k.Value.Value)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Otorgado");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Denegado");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("No especificado (Denegado)");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        private static void ShallowAudit(IProteusCredential credential)
        {
            Console.WriteLine($"Auditoría local de permisos para {credential.Name}");

            foreach (var j in Proteus.Services)
            {
                Console.WriteLine($"\n{j}\n---------------------------");
                foreach (var k in j.ShallowAudit(credential))
                {
                    PrintAudit(k);
                }
            }
        }
    }

    public class AboutMenuOption : MenuOption
    {
        public override string Summary => "Información del sistema";

        internal override void Run()
        {
            Helpers.About(GetType().Assembly);
            Separator();
            Helpers.About(TheXDS.MCART.Resources.RtInfo.CoreRtAssembly);
            Separator();
            Helpers.About(typeof(Proteus).Assembly);
            Separator();
            foreach (var j in Proteus.Services)
            {
                Helpers.About((IExposeInfo)j);
                Separator();
            }
        }
    }

    public class HeartbeatMenuOption : MenuOption
    {
        public override string Summary => "Efectuar Heartbeat";

        internal override void Run()
        {
            if (Client.SendHeartbeat())
            {
                Proteus.MessageTarget?.Info("El servidor ha respondido al comando Heartbeat.");
            }
            else
            {
                Proteus.MessageTarget?.Error("El servidor NO ha respondido al comando Heartbeat.");
            }
        }
    }

    public class UserQueryMenuOption : MenuOption
    {
        public override string Summary => "Listar usuarios conectados";

        internal override void Run()
        {
            foreach (var j in Client.QueryUsers())
            {
                Console.WriteLine($"{j.User} en {j.Host} ({j.Kind.NameOf()})");
            }
        }
    }

    public class EnumerateConnectionsMenuOption : MenuOption
    {
        public override string Summary => "Enumerar conexiones al servidor";

        internal override void Run()
        {
            var c = 0;
            foreach (var j in Client.EnumerateConnections())
            {
                Console.WriteLine($"  {c++}) {j.Address}{j.Port} ({j.HostId}) -> {j.UserId}, {j.Kind}");
            }
        }
    }

    public class AlertGenMenuOption : MenuOption
    {
        public override string Summary => "Generar mensaje de alerta";

        internal override void Run()
        {
            var msg = Input("Mensaje (Vacío para cancelar)");
            if (msg.IsEmpty()) return;
            Client.SendAlert(msg, InputList("Introduzca los canales de destino"), true);
        }
    }

    public class PurgeUserMenuOption : MenuOption
    {
        public override string Summary => "Purgar usuario";

        internal override void Run()
        {
            Client.SendPurge(InputList("Introduzca los usuarios a purgar"));
        }
    }

    public class DeafMenuOption : MenuOption
    {
        public override string Summary => "Modo sordo";

        internal override void Run()
        {
            Client.SendDeaf();
        }
    }

    public class ListenMenuOption : MenuOption
    {
        public override string Summary => "Salir del modo sordo";

        internal override void Run()
        {
            Client.SendListen();
        }
    }

    public class AnnounceMenuOption : MenuOption
    {
        public override string Summary => "Anunciar disponibilidad";

        internal override void Run()
        {
            Client.SendAnnounce();
        }
    }

    public class RunDaemonsMenuOption : MenuOption
    {
        public override string Summary => "Ejecutar manualmente los daemons del servidor";

        internal override void Run()
        {
            Client.SendRunDaemons(InputList("Introduzca los daemons a correr"));
        }
    }


    public class CloseMenuOption : MenuOption
    {
        public override string Summary => "Cerrar conexión específica";

        internal override void Run()
        {
            var i = Input("Introduzca el índice de la conexión a cerrar");
            if (short.TryParse(i, out var s))
            {
                Client.SendClose(s);
            }
        }
    }

    public class ShutdownMenuOption : MenuOption
    {
        public override string Summary => "Detener servidor";

        internal override void Run()
        {
            Client.SendShutdown();
        }
    }
}