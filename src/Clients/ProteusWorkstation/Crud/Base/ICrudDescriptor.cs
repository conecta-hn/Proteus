/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por una clase que provea
    /// de mecanismos de descripción para modelos de datos.
    /// </summary>
    public interface ICrudDescription
    {
        /// <summary>
        /// Obtiene una referencia al modelo de datos descrito poe este
        /// <see cref="ICrudDescription"/>.
        /// </summary>
        Type DescribedModel { get; }

        /// <summary>
        /// Obtiene un tipo base definido para la generación del ViewModel
        /// interno del editor de Crud, o <see langword="null"/> si no se
        /// ha definido un tipo base a implementar.
        /// </summary>
        Type BaseViewModelType { get; }

        /// <summary>
        /// Enumera a las descripciones definidas para las propiedades del
        /// modelo.
        /// </summary>
        IEnumerable<IPropertyDescription> Descriptions { get; }

        /// <summary>
        /// Obtiene una función definida a ejecutar previo a guardar una
        /// enitdad.
        /// </summary>
        IEnumerable<ICallSaveAction> BeforeSave { get; }

        /// <summary>
        /// Obtiene una función definida a ejecutar luego de guardar una
        /// enitdad.
        /// </summary>
        IEnumerable<ICallSaveAction> AfterSave { get; }

        /// <summary>
        /// Obtiene la función que determina si es posible eliminar a la
        /// entidad actualmente seleccionada.
        /// </summary>
        Func<ModelBase,bool>? CanDelete { get; }

        /// <summary>
        /// Enumera una serie de acciones personalizadas definidas para 
        /// mostrar en la venana del editor de Crud.
        /// </summary>
        IDictionary<string, Action<ModelBase, NotifyPropertyChangeBase>> CustomActions { get; }

        /// <summary>
        ///  Enumera las columnas a mostrar cuando el modelo sea presentado en 
        ///  un control <see cref="ListView"/>.
        /// </summary>
        IEnumerable<IColumn> ListColumns { get; }

        /// <summary>
        /// Obtiene un nombre amigable a utilizar para nomrbar al modelo.
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Obtiene un control personalizado de detalles a presentar.
        /// </summary>
        Control Details { get; }

        /// <summary>
        /// Plantilla a utilizar al presentar el modelo en un
        /// <see cref="TreeView"/>.
        /// </summary>
        DataTemplate? TreeViewTemplate { get; }

        /// <summary>
        /// Obtiene un valor que indica si el módulo debe cargar 
        /// automáticamente este descriptor en el menú.
        /// </summary>
        InteractionType? OnModuleMenu { get; }

        /// <summary>
        /// Obtiene un valor de determina si es posible crear nuevas 
        /// entidades.
        /// </summary>
        Func<ModelBase, bool>? CanCreate { get; }

        /// <summary>
        /// Obtiene un valor que determina si es posible editar una
        /// entidad.
        /// </summary>
        Func<ModelBase, bool>? CanEdit { get; }
        
        /// <summary>
        /// FUnción a ejecutar que comprueba una condición al abrir un Launcher.
        /// </summary>
        Func<bool>? LaunchPreCondition { get; }

        internal void SetCurrentEditor(ICrudEditingViewModel vm);
    }
}