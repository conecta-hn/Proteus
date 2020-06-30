/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Config
{
    /// <summary>
    /// Enumera las posibles acciones a tomar cuando ocurre un error de
    /// inicialización de la aplicación.
    /// </summary>
    public enum InitErrorActions : byte
    {
        /// <summary>
        /// Finalizar la aplicación.
        /// </summary>
        [Name("Finalizar aplicación")] Terminate,
        /// <summary>
        /// Abrir la configuración inicial.
        /// </summary>
        [Name("Iniciar configuración")] Config,
        /// <summary>
        /// Lanzar la excepción.
        /// </summary>
        [Name("Lanzar error al sistema")] Throw,
        /// <summary>
        /// Continuar normalmente la ejecución.
        /// </summary>
        [Name("Continuar (⚠ PELIGROSO)")] Continue,
#if DEBUG
        /// <summary>
        /// Iniciar el depurador.
        /// </summary>
        [Name("Iniciar depurador")] Debug = 255
#endif
    }
}