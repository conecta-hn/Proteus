/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Expone métodos de descripción de propiedades de enlace de datos
    /// externos para los cuales debe resolverse el origen.
    /// </summary>
    public interface ILinkPropertyDescription : IDataPropertyDescription, IListBasePropertyDescription
    {
        /// <summary>
        /// Obtiene una referencia al modelo descrito por el vínculo.
        /// </summary>
        Type Model { get; }
    }
}