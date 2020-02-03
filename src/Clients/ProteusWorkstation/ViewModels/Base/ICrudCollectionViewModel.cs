/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.ViewModels.Base
{

    /// <summary>
    /// Define una serie de miembros a implementar por una clase que
    /// administre las operaciones de Crud con colecciones con elementos de
    /// UI autogenerados.
    /// </summary>
    public interface ICrudCollectionViewModel : ICrudViewModel
    {
        /// <summary>
        /// Enumera el orígen de datos establecido para este Crud.
        /// </summary>
        ICollection<ModelBase> Source { get; }

        /// <summary>
        /// Obtiene al elemento selector de la ventana.
        /// </summary>
        ItemsControl Selector { get; }

        /// <summary>
        /// Obtiene un <see cref="ViewBase"/> que define la apariencia de
        /// un selector <see cref="ListView"/> cuando esta ventana de CRUD
        /// controla únicamente un modelo de datos.
        /// </summary>
        ViewBase? ColumnsView { get; }
    }
}