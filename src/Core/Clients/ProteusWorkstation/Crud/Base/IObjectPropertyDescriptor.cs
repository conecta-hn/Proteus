/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    ///     Expone métodos de descripción de propiedades de enlace de datos 1-N
    /// </summary>
    public interface IObjectPropertyDescriptor : IDataPropertyDescriptor
    {
        /// <summary>
        ///     Indica que el control generado para la propiedad debe permitir
        ///     seleccionar una entidad de datos desde una lista.
        /// </summary>
        /// <returns>
        ///     Una referencia a la misma instancia para utilizar sintáxis
        ///     Fluent.
        /// </returns>
        IObjectPropertyDescriptor Selectable();
    }
}