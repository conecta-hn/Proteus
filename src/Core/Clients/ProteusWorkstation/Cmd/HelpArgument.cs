/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.ViewModels;
using System.Text;
using TheXDS.MCART.Component;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Cmd
{
    public class HelpArgument : Argument
    {
        public override string Summary => "Muestra la ayuda sobre la línea de comandos de esta aplicación.";
        public override void Run(CmdLineParser args)
        {
            MainWindowViewModel._exiting = true;

            if (WinUi.HasConsole)
            {
                args.PrintHelp();
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var j in args.AvailableArguments)
                {
                    sb.AppendLine(DescribeArg(j));
                    sb.AppendLine(j.Summary);
                    sb.AppendLine();
                }
                App.UiInvoke(() => MessageSplash.Show("Ayuda de la línea de comandos", sb.ToString(), MessageType.Info));
            }
            App.UiInvoke(() => ((App.RootHost as MainWindowViewModel)?.Host as MainWindow)?.ForceClose());
        }
        private static string DescribeArg(Argument arg)
        {
            return arg.Kind switch
            {
                ValueKind.Optional => $"[--{arg.LongName}[=valor]]",
                ValueKind.ValueRequired => $"[--{arg.LongName}=valor]",
                ValueKind.Required => $"--{arg.LongName}=valor",
                _ => $"[--{arg.LongName}]"
            };
        }
    }
}