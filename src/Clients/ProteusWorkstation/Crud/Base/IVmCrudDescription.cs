/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por una clase que provea
    /// de mecanismos de descripción para modelos de datos con acceso a
    /// descriptores de ViewModel.
    /// </summary>
    public interface IVmCrudDescription : ICrudDescription
    {
        /// <summary>
        /// Obtiene una función definida a ejecutar previo a guardar una
        /// enitdad controlada por un ViewModel.
        /// </summary>
        IEnumerable<ICallSaveAction> VmBeforeSave { get; }

        /// <summary>
        /// Obtiene una función definida a ejecutar luego de guardar una
        /// enitdad controlada por un ViewModel.
        /// </summary>
        IEnumerable<ICallSaveAction> VmAfterSave { get; }
    }
}