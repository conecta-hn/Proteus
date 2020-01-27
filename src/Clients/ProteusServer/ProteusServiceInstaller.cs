/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

/*
using System.ServiceProcess;
using System.ComponentModel;
using System.Configuration.Install;

namespace TheXDS.Proteus
{
    [RunInstaller(true)]
    public class ProteusServiceInstaller : Installer
    {
        public ProteusServiceInstaller()
        {            
            Installers.AddRange(new Installer[]
            {
                new ServiceProcessInstaller
                {
                    Account = ServiceAccount.NetworkService,
                    Password = null,
                    Username = null
                },
                new ServiceInstaller
                {
                    ServiceName = "ProteusService",
                    StartType = ServiceStartMode.Automatic,
                    Description = "Provee de servicios de red a los clientes del sistema Proteus. Si se detiene o se deshabilita este servicio, los clientes no podrán participar en el subsistema de gestión de sesión ni podrán intercambiar mensajes entre sí."
                }
            });
        }
    }
}
*/