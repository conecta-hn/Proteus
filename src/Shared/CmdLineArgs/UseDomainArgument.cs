/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;
using TheXDS.MCART.Exceptions;

namespace TheXDS.Proteus.Cmd
{
    public class UseDomainArgument : ProteusArgument
    {
        public override ValueKind Kind => ValueKind.Optional;

        public override string Summary => "Cambia el método de conexión de Proteus para conectarse al servidor SQL especificado y utilizar credenciales LDAP integradas de Active Directory.";

        public override void Run(CmdLineParser args)
        {
            if (!(Value is null) || string.IsNullOrWhiteSpace(Value))
            {
            
                Settings.Default.UseLocalDbProvider = false;
                Settings.Default.UseDomainProvider = true;
                Settings.Default.UseCustomProvider = false;
                Settings.Default.DomainProvider = Value;
            }
            else
            {
                InvalidArg();
            }
        }
    }
}