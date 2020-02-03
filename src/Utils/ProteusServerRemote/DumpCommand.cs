/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Component
{
    public enum DumpCommand : byte
    {
        /// <summary>
        ///     De forma predeterminada, el servicio envía los datos en formato
        ///     compatible con Telnet. Este comando configura la conexión para
        ///     usar el modo mejorado con soporte para tipos de mensajes.
        /// </summary>
        Enhance,

        /// <summary>
        ///     Finaliza la sesión de consola remota.
        /// </summary>
        Disconnect = 113
    }
}