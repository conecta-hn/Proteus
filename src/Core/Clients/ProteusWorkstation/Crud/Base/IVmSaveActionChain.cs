/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// concatenar acciones de guardado.
    /// </summary>
    /// <typeparam name="TModel">
    /// Tipo de entidad de la acción de guardado.
    /// </typeparam>
    /// <typeparam name="TViewModel">
    /// Tipo de ViewModel a pasar como parámetro a las funciones de
    /// guardado.
    /// </typeparam>
    public interface IVmSaveActionChain<TModel, TViewModel> : ICallSaveAction where TModel : ModelBase, new() where TViewModel : class, IDynamicViewModel<TModel>
    {
        /// <summary>
        /// Concatena una acción de guardado.
        /// </summary>
        /// <param name="action">
        /// Acción a concatenar.
        /// </param>
        /// <returns>
        /// Esta misma instancia.
        /// </returns>
        IVmSaveActionChain<TModel, TViewModel> Then(Action<TViewModel> action);

        /// <summary>
        /// Concatena una acción de guardado.
        /// </summary>
        /// <param name="action">
        /// Acción a concatenar.
        /// </param>
        /// <returns>
        /// Esta misma instancia.
        /// </returns>
        IVmSaveActionChain<TModel, TViewModel> Then(Action<TViewModel, ModelBase> action);
    }
}