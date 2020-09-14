/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class ChDirArgument : ProteusArgument
    {
        public override ValueKind Kind => ValueKind.ValueRequired;
        public override string Summary => "Cambia de directorio antes de ejecutar la aplicación.";
        public override void Run(CmdLineParser args)
        {
            try
            {
                Environment.CurrentDirectory = Value!;
            }
            catch (Exception ex)
            {
                ShowWarning("No se pudo cambiar el directorio", ex.Message);
            }
        }
    }
}