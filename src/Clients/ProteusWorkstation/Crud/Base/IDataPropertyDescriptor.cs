/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Expone métodos de descripción para todas las propiedades de enlace 
    /// a datos.
    /// </summary>
    public interface IDataPropertyDescriptor : IPropertyDescriptor
    {
        /// <summary>
        /// Indica que se debe utilizar un origen específico para la
        /// obtención de datos.
        /// </summary>
        /// <param name="source">
        /// Origen de datos a utilizar.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IDataPropertyDescriptor Source(IQueryable<ModelBase> source);

        /// <summary>
        /// Indica que el control generado para la propiedad debe permitir
        /// crear nuevas entidades.
        /// </summary>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IDataPropertyDescriptor Creatable();

        /// <summary>
        /// Indica que el control generado para la propiedad debe permitir
        /// crear nuevas entidades.
        /// </summary>
        /// <param name="models">
        /// Modelos que pueden ser creados.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IDataPropertyDescriptor Creatable(IEnumerable<Type> models);

        /// <summary>
        /// Indica que se utilizará una propiedad del objeto para su
        /// presentación visual.
        /// </summary>
        /// <param name="selector">
        /// Selector de la propiedad a utilizar para presentar el objeto 
        /// visualmente.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IDataPropertyDescription DisplayMember(Expression<Func<ModelBase, object>> selector);

        /// <summary>
        /// Indica que se utilizará una propiedad del objeto para su
        /// presentación visual.
        /// </summary>
        /// <param name="path">
        /// Ruta en formato <see cref="string"/> que será pasada
        /// directamente al <see cref="System.Windows.Data.Binding"/> 
        /// resultante.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IDataPropertyDescription DisplayMember(string path);
    }
}