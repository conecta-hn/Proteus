/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public partial class Alerta : ITitledText
    {
        public Alerta() : this("Alerta", null, null, null, null) { }
        public Alerta(string header) : this(header, null, null, null, null) { }
        public Alerta(string header, string? body) : this(header, body, null, null, null) { }
        public Alerta(string header, Action<Alerta>? interaction) : this(header, interaction, null) { }
        public Alerta(string header, Action<Alerta>? interaction, object? morInfo) : this(header, null, interaction, null, morInfo) { }
        public Alerta(string header, string? body, Action<Alerta>? interaction, ICollection<Alerta>? parent) : this(header, body, interaction, parent, null) { }
        public Alerta(string header, string? body, Action<Alerta>? interaction, ICollection<Alerta>? parent, object? morInfo)
        {
            Header = header;
            Body = $"Alerta generada el {Timestamp}\n\n{body}";
            Action = interaction;
            Parent = parent;
            MorInfo = morInfo;
        }

        /// <summary>
        /// Instante de creación de la alerta.
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.Now;

        /// <summary>
        /// Contenido textual extendido de la alerta.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Encabezado textual de la alerta.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Datos especiales a incluir en la alerta.
        /// </summary>
        public object? MorInfo { get; }

        /// <summary>
        /// Acción a ejecutar en la alerta.
        /// </summary>
        public Action<Alerta>? Action { get; }

        public bool HasInteraction => !(Action is null);

        internal ICollection<Alerta>? Parent { get; }

        public void Dismiss() => Parent?.Remove(this);
    }
}