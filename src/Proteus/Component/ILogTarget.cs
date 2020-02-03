/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Describe una serie de miembros a implementar por una clase que 
    /// acepte entradas de bitácora de eventos ocurridos dentro de la
    /// aplicación.
    /// </summary>
    public interface ILogTarget
    {
        /// <summary>
        /// Escribe un texto que representa un evento en la bitácora.
        /// </summary>
        /// <param name="text">
        /// Texo a escribir en la bitácora.
        /// </param>
        void Log(string text);
    }
}