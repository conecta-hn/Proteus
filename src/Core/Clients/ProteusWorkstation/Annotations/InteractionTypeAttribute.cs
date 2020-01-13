/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Annotations
{
    /// <summary>
    /// Atributo que describe el tipo de interacción que un método es.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InteractionTypeAttribute : Attribute, IValueAttribute<InteractionType>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="InteractionTypeAttribute"/>.
        /// </summary>
        /// <param name="value">
        /// Tipo de interacción de este elemento.
        /// </param>
        public InteractionTypeAttribute(InteractionType value)
        {
            Value = value;
        }

        /// <summary>
        /// Obtiene el tipo de interacción de este elemento.
        /// </summary>
        public InteractionType Value { get; }
    }

    /// <summary>
    /// Enumera los distintos tipos de interacción disponibles de forma
    /// predeterminada en Proteus.
    /// </summary>
    [Flags]
    public enum InteractionType : byte
    {
        /// <summary>
        /// Marca la interacción como una ventana importante.
        /// </summary>
        [Name("Inicio")]
        Essential = 1,
        /// <summary>
        /// Marca la interacción como una ventana administrativa.
        /// </summary>
        [Name("Ventanas administrativas")]
        AdminTool = 2,
        /// <summary>
        /// Marca la interacción como una ventana de operación.
        /// </summary>
        [Name("Operaciones")]
        Operation = 4,
        /// <summary>
        /// Marca la interacción como un reporte.
        /// </summary>
        [Name("Reportes")]
        Reports = 8,
        /// <summary>
        /// Marca la interacción como una ventana de configuración.
        /// </summary>
        [Name("Configuración")]
        Settings = 16,
        /// <summary>
        /// Marca la interacción como una herramienta.
        /// </summary>
        [Name("Herramientas")]
        Tool = 32,
        /// <summary>
        /// Marca la interacción como una ventana de administración de catálogo.
        /// </summary>
        [Name("Catálogo")]
        Catalog = 64,
        /// <summary>
        /// Herramientas misceláneas
        /// </summary>
        [Name("Herramientas misceláneas")]
        Misc = 128
    }
}