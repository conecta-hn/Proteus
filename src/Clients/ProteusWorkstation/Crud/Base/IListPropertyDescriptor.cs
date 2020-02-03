/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Describe las propiedades de lista contenidas dentro de la misma 
    /// base de datos.
    /// </summary>
    public interface IListPropertyDescriptor<T> : IObjectPropertyDescriptor, IListBasePropertyDescriptor<IListPropertyDescriptor<T>,T> where T : ModelBase
    {
        /// <summary>
        /// Indica que el valor asociado a este descriptor debe ser 
        /// editable.
        /// </summary>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IListPropertyDescriptor<T> Editable();
    }
}