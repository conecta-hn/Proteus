/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    ///     Expone las descripciones de la propiedad realizadas por medio de la
    ///     interfaz <see cref="IObjectPropertyDescriptor"/>.
    /// </summary>
    public interface IObjectPropertyDescription : IDataPropertyDescription
    {
        /// <summary>
        ///     Indica si la propiedad fue marcada como seleccionable.
        /// </summary>
        bool Selectable { get; }
    }
}