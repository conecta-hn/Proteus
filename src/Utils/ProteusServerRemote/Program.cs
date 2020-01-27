/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using System;
using System.Diagnostics;
using System.Threading;
using TheXDS.MCART.Cmd;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Remote
{
    public class UniThreadArgument : Argument
    {
        public override string LongName => "UniThread";
        public override char? ShortName => 'u';
        public override string Summary => "Establece la afinidad de CPU de esta aplicación para utilizar únicamente el último procesador lógico del sistema.";
        public override void Run(CmdLineParser args)
        {
            try
            {
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << (Environment.ProcessorCount - 1));
                Program.MessageTarget?.Info($"Modo de afinidad uni-proceso. Utilizando procesador lógico {Environment.ProcessorCount - 1}");
            }
            catch (Exception ex)
            {
                Program.MessageTarget?.Warning($"No se pudo establecer la afinidad del proceso: {ex.Message}");
            }
        }
    }

    public class LowPriorityArgument : Argument
    {
        public override char? ShortName => 'l';
        public override string Summary => "Establece la prioridad del proceso de esta aplicación en la más baja.";
        public override void Run(CmdLineParser args)
        {
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                Program.MessageTarget?.Info("Modo de proceso de baja prioridad.");
            }
            catch (Exception ex)
            {
                Program.MessageTarget?.Warning($"No se pudo establecer la prioridad del proceso: {ex.Message}");
            }
        }
    }

    public class HelpArgument : HelpArgumentBase
    {
        public override string Summary => "Obtiene la ayuda de este programa.";
    }

    public class Telnet : Argument
    {
        public override string Summary => "Indica a la terminal remota que funcionará en modo de compatilibidad con Telnet al no enviar un comando de mejora de sesión.";

        public override char? ShortName => 't';
    }

    public class Server : Argument
    {
        public override char? ShortName => 's';
        public override string Default => "localhost";
        public override ValueKind Kind => ValueKind.ValueRequired;
        public override string Summary => "Indica el nombre del servidor al cual se realizará la conexión.";
    }

    public class Port : Argument
    {
        public override char? ShortName => 'p';
        public override string Default => 51201.ToString();
        public override ValueKind Kind => ValueKind.ValueRequired;
        public override string Summary => "Indica número de puerto del servidor al cual se realizará la conexión.";
    }

    static class Program
    {
        public static ConsoleMessageTarget MessageTarget { get; } = new ConsoleMessageTarget();
        static void Main(string[] args)
        {

            var a = new CmdLineParser(Environment.GetCommandLineArgs());
            a.AutoRun(true);
            if (a.IsPresent<HelpArgument>())
            {
                Environment.Exit(0);
            }

            var c = new RemoteTerminalClient(MessageTarget);
            if (!c.Connect(a.Value<Server>() ?? "localhost", int.TryParse(a.Value<Port>(), out var prt) ? prt : 51201))
            {
                MessageTarget.Error($"No fue posible conectarse al servidor {a.Value<Server>() ?? "localhost"}:{prt}.");
                return;
            }
            else
            {
                MessageTarget.Show("Conexión establecida correctamente.");
            }
            if (!a.IsPresent<Telnet>()) c.Enhance();
            Console.CancelKeyPress += (_, e) => Environment.Exit(0);
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}