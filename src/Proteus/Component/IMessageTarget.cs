/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Describe una serie de miembros a implementar por una clase que 
    /// acepte mensajes de eventos ocurridos dentro de la aplicación.
    /// </summary>
    public interface IMessageTarget
    {
        /// <summary>
        /// Notifica de un mensaje simple.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        void Show(string message);

        /// <summary>
        /// Notifica de un mensaje simple.
        /// </summary>
        /// <param name="title">
        /// Título del mensaje.
        /// </param>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        void Show(string title, string message);

        /// <summary>
        /// Notifica de un mensaje informativo.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        void Info(string message);

        /// <summary>
        /// Notifica de una advertencia.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        void Warning(string message);

        /// <summary>
        /// Notifica de un mensaje producido por una operación que debe
        /// detenerse.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        void Stop(string message);

        /// <summary>
        /// Notifica de un mensaje de error.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        void Error(string message);

        /// <summary>
        /// Notifica de un error crítico.
        /// </summary>
        /// <param name="message">
        /// Mensaje a enviar al objetivo.
        /// </param>
        void Critical(string message);

        /// <summary>
        /// Notifica de un error crítico.
        /// </summary>
        /// <param name="ex">
        /// Excepción producida a notificar.
        /// </param>
        void Critical(Exception ex);
    }

    /// <summary>
    /// Describe una serie de miembros a implementar por una clase que 
    /// acepte mensajes de eventos ocurridos dentro de la aplicación además de
    /// ofrecer ciertos servicios básicos de interactividad.
    /// </summary>
    public interface IInteractiveMessageTarget : IMessageTarget
    {
        /// <summary>
        /// Realiza una pregunta al usuario.
        /// </summary>
        /// <param name="title">Título del mensaje.</param>
        /// <returns>
        /// <see langword="true"/> si el usuario ha dicho que sí al cuadro
        /// de diálogo, <see langword="false"/> en caso contrario.
        /// </returns>
        bool Ask(string title)
        {
            return Ask(title, "¿Está seguro que desea realizar esta operación?");
        }

        /// <summary>
        /// Realiza una pregunta al usuario.
        /// </summary>
        /// <param name="title">Título del mensaje.</param>
        /// <param name="message">Contenido de la pregunta.</param>
        /// <returns>
        /// <see langword="true"/> si el usuario ha dicho que sí al cuadro
        /// de diálogo, <see langword="false"/> en caso contrario.
        /// </returns>
        bool Ask(string title, string message);
    }
}