/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     concatenar acciones de guardado.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de entidad de la acción de guardado.
    /// </typeparam>
    public interface ISaveActionChain<T> : ICallSaveAction where T : ModelBase
    {
        /// <summary>
        ///     Concatena una acción de guardado.
        /// </summary>
        /// <param name="action">
        ///     Acción a concatenar.
        /// </param>
        /// <returns>
        ///     Esta misma instancia.
        /// </returns>
        ISaveActionChain<T> Then(Action<T> action);

        /// <summary>
        ///     Concatena una acción de guardado.
        /// </summary>
        /// <param name="action">
        ///     Acción a concatenar.
        /// </param>
        /// <returns>
        ///     Esta misma instancia.
        /// </returns>
        ISaveActionChain<T> Then(Action<T, ModelBase> action);
    }
}