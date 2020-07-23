/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Component
{
    public abstract class ModelSearchFilterBase<T>
    {

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
        public bool UsableFor(Type model) => model.Implements<T>();
    }
}