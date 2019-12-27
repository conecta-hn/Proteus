/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Component
{
    /// <summary>
    ///     Enumera las respuestas enviadas por el protocolo Dump.
    /// </summary>
    public enum DumpResponse : byte
    {
        /// <summary>
        ///     Indica que el mensaje es un error crítico.
        /// </summary>
        Critical,
        /// <summary>
        ///     Indica que el mensaje indica la finalización de una tarea.
        /// </summary>
        Done,
        /// <summary>
        ///     Indica que el mensaje es un error genérico.
        /// </summary>
        Error,
        /// <summary>
        ///     Indica que el mensaje es un mensaje informativo.
        /// </summary>
        Info,
        /// <summary>
        ///     Indica que el mensaje es un error de detención.
        /// </summary>
        Stop,
        /// <summary>
        ///     Indica que el mensaje es un reporte de operación.
        /// </summary>
        Report,
        /// <summary>
        ///     Indica que el mensaje es una advertencia.
        /// </summary>
        Warning,
        /// <summary>
        ///     Indica que el mensaje es un mensaje común. Por razones de
        ///     compatilibidad con Telnet, se establece en el caracter '#'.
        /// </summary>
        Message = 35
    }
}