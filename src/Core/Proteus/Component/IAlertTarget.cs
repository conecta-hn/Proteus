/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models;
using System;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Describe una serie de miembros a implementar por una clase que
    /// permita mostrar alertas emergentes al usuario.
    /// </summary>
    public interface IAlertTarget
    {
        /// <summary>
        /// Muestra una alerta que consiste únicamente en un mensaje.
        /// </summary>
        /// <param name="alert">Mensaje de alerta a mostrar.</param>
        void Alert(string alert);

        /// <summary>
        /// Muestra una alerta que consiste en un mensaje con título.
        /// </summary>
        /// <param name="header">
        /// Título de la alerta.
        /// </param>
        /// <param name="body">Contenido de la alerta.</param>
        void Alert(string header, string body);

        /// <summary>
        /// Muestra una alerta que contiene un mensaje y una acción
        /// interactiva.
        /// </summary>
        /// <param name="alert">Mensaje de alerta a mostrar.</param>
        /// <param name="interaction">
        /// Acción a ejecutar al interactuar con la alerta.
        /// </param>
        void Alert(string alert, Action<Alerta> interaction);

        /// <summary>
        /// Muestra una alerta que contiene un mensaje con título y cuerpo,
        /// además de una acción interactiva.
        /// </summary>
        /// <param name="header">
        /// Título de la alerta.
        /// </param>
        /// <param name="body">Contenido de la alerta.</param>
        /// <param name="interaction">
        /// Acción a ejecutar al interactuar con la alerta.
        /// </param>
        void Alert(string header, string body, Action<Alerta> interaction);

        /// <summary>
        /// Muestra una alerta que contiene un mensaje y una acción
        /// interactiva.
        /// </summary>
        /// <param name="alert">Mensaje de alerta a mostrar.</param>
        /// <param name="interaction">
        /// Acción a ejecutar al interactuar con la alerta.
        /// </param>
        /// <param name="morInfo">
        /// Objeto que contiene información adicional sobre la alerta ocurrida.
        /// </param>
        void Alert(string alert, Action<Alerta> interaction, object morInfo);

        /// <summary>
        /// Muestra una alerta que contiene un mensaje con título y cuerpo,
        /// además de una acción interactiva.
        /// </summary>
        /// <param name="header">
        /// Título de la alerta.
        /// </param>
        /// <param name="body">Contenido de la alerta.</param>
        /// <param name="interaction">
        /// Acción a ejecutar al interactuar con la alerta.
        /// </param>
        /// <param name="morInfo">
        /// Objeto que contiene información adicional sobre la alerta ocurrida.
        /// </param>
        void Alert(string header, string body, Action<Alerta> interaction, object morInfo);
    }
}