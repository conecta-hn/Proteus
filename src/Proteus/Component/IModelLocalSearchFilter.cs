/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Component
{
    public interface IModelLocalSearchFilter
    {
        /// <summary>
        /// Obtiene una lista de entidades filtradas por esta instancia.
        /// </summary>
        /// <typeparam name="TModel">Modelo que este filtro acepta.</typeparam>
        /// <param name="collection">Colección de entidades de entrada.</param>
        /// <param name="query">Cadena de filtro.</param>
        /// <returns>
        /// Un <see cref="List{T}"/> con las entidades que han cumplido con las
        /// condiciones de este filtro de búsqueda local.
        /// </returns>
        List<TModel> Filter<TModel>(List<TModel> collection, string query) where TModel : ModelBase;

        /// <summary>
        /// Comprueba si este <see cref="IModelSearchFilter"/> puede crear
        /// filtros para el modelo especificado.
        /// </summary>
        /// <param name="model">Modelo a comprobar.</param>
        /// <returns>
        /// <see langword="true"/> si esta instancia puede crear filtros de
        /// consulta para el modelo especificado, <see langword="false"/>
        /// en caso contrario.
        /// </returns>
        bool UsableFor(Type model);
    }
}