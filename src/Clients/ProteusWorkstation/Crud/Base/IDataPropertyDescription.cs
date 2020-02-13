/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;
using System.Linq;
using System.Windows.Data;
using TheXDS.MCART.Types;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Expone las descripciones de la propiedad realizadas por medio de la
    /// interfaz <see cref="IDataPropertyDescriptor"/>.
    /// </summary>
    public interface IDataPropertyDescription : IPropertyDescription
    {
        /// <summary>
        /// Obtiene un <see cref="BindingBase"/> por medio del cual se
        /// representará visualmente a la entidad relacionada.
        /// </summary>
        BindingBase DisplayMemberBinding { get; }

        /// <summary>
        /// Obtiene una ruta de Binding por medio de la cual se
        /// representará visualmente a la entidad relacionada.
        /// </summary>
        string DisplayMemberPath { get; }

        /// <summary>
        /// Obtiene el origen de datos definido para la propiedad.
        /// </summary>
        IQueryable<ModelBase> Source { get; }

        bool UseVmSource { get; }

        ObservableListWrap<ModelBase>? VmSource(object parentVm) => VmSource(parentVm, null);
        ObservableListWrap<ModelBase>? VmSource(object parentVm, CrudViewModelBase? elementVm);

        /// <summary>
        /// Indica si la propiedad fue marcada para poder crear una nueva
        /// instancia de la entidad relacionada.
        /// </summary>
        bool Creatable { get; }

        /// <summary>
        /// Enumera los modelos que pueden ser creados o seleccionados
        /// utilizando los controles generados de la UI.
        /// </summary>
        IEnumerable<Type> ChildModels { get; }
    }
}