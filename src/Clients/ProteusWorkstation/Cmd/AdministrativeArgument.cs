/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Component;
using TheXDS.Proteus.Misc;

namespace TheXDS.Proteus.Cmd
{
    /// <summary>
    /// Describe un argumento que requiere de permisos administrativos para ejecutarse.
    /// </summary>
    public abstract class AdministrativeArgument : Argument
    {
        public override sealed void Run(CmdLineParser args)
        {
            if (!AppInternal.IsAdministrator())
            {
                var m = $"{LongName}: Esta operación solo puede ser realizada por un administrador.";
                if (Proteus.AlertTarget is null)
                {
                    Proteus.MessageTarget?.Info(m);
                }
                else
                {
                    Proteus.AlertTarget.Alert(m);
                }
            }
            else OnRun(args);
        }
        protected abstract void OnRun(CmdLineParser args);
    }
}