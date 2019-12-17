/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Linq.Expressions;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    ///     Expone métodos de descripción para una propiedad que representa un
    ///     vínculo a un objeto externo al contexto de datos del modelo actual.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de objeto externo representado por esta propiedad.
    /// </typeparam>
    public interface ILinkPropertyDescriptor<T> : IDataPropertyDescriptor, IListBasePropertyDescriptor<ILinkPropertyDescriptor<T>,T> where T : ModelBase
    {
        /// <summary>
        ///     Indica que se utilizará una propiedad del objeto para su
        ///     presentación visual.
        /// </summary>
        /// <param name="selector">
        ///     Selector de la propiedad a utilizar para presentar el objeto 
        ///     visualmente.
        /// </param>
        /// <returns>
        ///     Una referencia a la misma instancia para utilizar sintáxis
        ///     Fluent.
        /// </returns>
        ILinkPropertyDescriptor<T> DisplayMember(Expression<Func<T, object>> selector);

        /// <summary>
        ///     Indica que se utilizará una propiedad del objeto para su
        ///     presentación visual.
        /// </summary>
        /// <param name="path">
        ///     Ruta en formato <see cref="string"/> que será pasada
        ///     directamente al <see cref="System.Windows.Data.Binding"/> 
        ///     resultante.
        /// </param>
        /// <returns>
        ///     Una referencia a la misma instancia para utilizar sintáxis
        ///     Fluent.
        /// </returns>
        new ILinkPropertyDescriptor<T> DisplayMember(string path);
    }
}