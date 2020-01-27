/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que describa
    /// las propiedades asociadas a una lista dentro de un modelo de datos.
    /// </summary>
    public interface IListPropertyDescription : IObjectPropertyDescription, IListBasePropertyDescription
    {
        /// <summary>
        /// Obtiene un valor que indica si los elementos de la lista son
        /// editables.
        /// </summary>
        bool Editable { get; }
    }
}