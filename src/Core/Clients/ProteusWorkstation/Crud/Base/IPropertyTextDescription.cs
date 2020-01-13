/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Expone métodos de descripción de propiedades de texto.
    /// </summary>
    public interface IPropertyTextDescription : IPropertyDescription
    {
        /// <summary>
        /// Indica que el control manejará grandes cantidades de texto, por
        /// lo que debe ser de gran tamaño.
        /// </summary>
        TextKind Kind { get; }
        /// <summary>
        /// Obtiene la longitud mínima de texto para el control.
        /// </summary>
        int MinLength { get; }
        /// <summary>
        /// Obtiene la longitud máxima establecida en el control.
        /// </summary>
        int MaxLength { get; }
        /// <summary>
        /// Obtiene una máscara establecida para el campo de texto.
        /// </summary>
        string Mask { get; }
    }
}