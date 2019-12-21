/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.ViewModels;
using System.Windows;
using TheXDS.MCART;
using TheXDS.MCART.Component;
using TheXDS.MCART.Dialogs;
using TheXDS.MCART.Cmd;

namespace TheXDS.Proteus.Cmd
{
    public class VersionArgument : Argument
    {
        public override string Summary => "Muestra la versión de esta aplicación.";
        public override void Run(CmdLineParser args)
        {
            MainWindowViewModel._exiting = true;

            if (WinUi.HasConsole)
            {
                Helpers.About();
            }
            else
            {
                App.UiInvoke(() => AboutBox.ShowDialog(Application.Current));
            }
            App.UiInvoke(App.RootHost.ForceClose);
        }
    }
}