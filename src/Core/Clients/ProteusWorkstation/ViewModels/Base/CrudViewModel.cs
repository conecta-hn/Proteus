/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TheXDS.Proteus.Widgets;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Crud;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    ///     ViewModel que controla la gestión de la ventana de Crud.
    /// </summary>
    /// <typeparam name="TService">
    ///     Tipo de servicio enlazado.
    /// </typeparam>
    public class CrudViewModel<TService> : PageViewModel, ICrudCollectionViewModel where TService : Service, new()
    {
        /// <summary>
        ///     Obtiene un valor que indica si este ViewModel se encuentra 
        ///     ocupado.
        /// </summary>
        public new bool IsBusy => Implementation.IsBusy;

        private CrudCollectionViewModelBase Implementation { get; }

        /// <summary>
        ///     Obtiene al elemento selector de la ventana.
        /// </summary>
        public ItemsControl Selector => ((ICrudCollectionViewModel)Implementation).Selector;

        /// <summary>
        ///     Obtiene o establece al elemento seleccionado.
        /// </summary>
        public object Selection { get => ((ICrudCollectionViewModel)Implementation).Selection; set => ((ICrudCollectionViewModel)Implementation).Selection = value; }

        /// <summary>
        ///     Obtiene un <see cref="ViewBase"/> que define la apariencia de
        ///     un selector <see cref="ListView"/> cuando esta ventana de CRUD
        ///     controla únicamente un modelo de datos.
        /// </summary>
        public ViewBase ColumnsView => ((ICrudCollectionViewModel)Implementation).ColumnsView;

        /// <summary>
        ///     Enumera el orígen de datos establecido para este Crud.
        /// </summary>
        public ICollection<ModelBase> Source => ((ICrudCollectionViewModel)Implementation).Source;

        /// <summary>
        ///     Obtiene la ventana de detalles de la entidad seleccionada.
        /// </summary>
        public FrameworkElement SelectedDetails => ((ICrudCollectionViewModel)Implementation).SelectedDetails;

        /// <summary>
        ///     Obtiene el editor a utlizar para editar a la entidad seleccionada.
        /// </summary>
        public FrameworkElement SelectedEditor => ((ICrudCollectionViewModel)Implementation).SelectedEditor;

        /// <summary>
        ///     Obtiene un <see cref="CrudElement"/> con información sobre los
        ///     componentes relacionados al modelo de datos de la entidad
        ///     seleccionada.
        /// </summary>
        public CrudElement SelectedElement => ((ICrudCollectionViewModel)Implementation).SelectedElement;

        /// <summary>
        ///     Comando para la creación de nuevas entidades.
        /// </summary>
        public ICommand CreateNew => ((ICrudCollectionViewModel)Implementation).CreateNew;

        /// <summary>
        ///     Comando para la edición de la entidad actualmente seleccionada.
        /// </summary>
        public ICommand EditCurrent => ((ICrudCollectionViewModel)Implementation).EditCurrent;

        /// <summary>
        ///     Comando para la eliminación de la entidad actualmente
        ///     seleccionada.
        /// </summary>
        public ICommand DeleteCurrent => ((ICrudCollectionViewModel)Implementation).DeleteCurrent;

        /// <summary>
        ///     Comando de guardado de entidades, tanto nuevas como editadas.
        /// </summary>
        public ICommand SaveCommand => ((ICrudCollectionViewModel)Implementation).SaveCommand;

        /// <summary>
        ///     Comando que cancela la creación o edición de una entidad.
        /// </summary>
        public ICommand CancelCommand => ((ICrudCollectionViewModel)Implementation).CancelCommand;

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable mientras el ViewModel se encuentre ocupado.
        /// </summary>
        public Visibility BusyV => ((ICrudCollectionViewModel)Implementation).BusyV;

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable mientras el ViewModel no se encuentre ovupado.
        /// </summary>
        public Visibility NotBusyV => ((ICrudCollectionViewModel)Implementation).NotBusyV;

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable cuando el ViewModel administre la creación de múltiples modelos.
        /// </summary>
        public Visibility MultiModel => ((ICrudCollectionViewModel)Implementation).MultiModel;

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicacble cuando el ViewModel administre la creación de un único modelo.
        /// </summary>
        public Visibility UniModel => ((ICrudCollectionViewModel)Implementation).UniModel;

        /// <summary>
        ///     Obtiene un valor que indica si el ViewModel se encuentra actualmente en modo de edición.
        /// </summary>
        public bool EditMode => ((ICrudCollectionViewModel)Implementation)?.EditMode ?? false;

        /// <summary>
        ///     Obtiene un valor que indica si el ViewModel no se encuentra actualmente en modo de edición.
        /// </summary>
        public bool NotEditMode => ((ICrudCollectionViewModel)Implementation).NotEditMode;

        /// <summary>
        ///     Obtiene un valor que indica si el ViewModel no está ocupado.
        /// </summary>
        public bool NotBusy => ((ICrudCollectionViewModel)Implementation).NotBusy;

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable cuando el ViewModel se encuentre en modo de edición.
        /// </summary>
        public Visibility EditVis => ((ICrudCollectionViewModel)Implementation).EditVis;

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable cuando el ViewModel no se encuentre en modo de edición.
        /// </summary>
        public Visibility NotEditVis => ((ICrudCollectionViewModel)Implementation).NotEditVis;

        /// <summary>
        ///     Enumeración de comandos para la creación de entidades cuando
        ///     este ViewModel administra dos o más modelos de datos.
        /// </summary>
        public IEnumerable<Launcher> CreateCommands => ((ICrudCollectionViewModel)Implementation).CreateCommands;

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="CrudViewModel{TService}"/>.
        /// </summary>
        /// <param name="host">
        ///     Host visual de la ventana asociada a este ViewModel.
        /// </param>
        /// <param name="model">
        ///     Modelo único de datos.
        /// </param>
        public CrudViewModel(ICloseable host, Type model) : base(host)
        {
            Implementation = new DbBoundCrudViewModel(model);
            Implementation.ForwardChange(this);

            RegisterPropertyChangeTrigger(nameof(Title),
                nameof(EditMode));

            RegisterPropertyChangeBroadcast(nameof(EditMode), nameof(Closeable));
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="CrudViewModel{TService}"/>.
        /// </summary>
        /// <param name="host">
        ///     Host visual de la ventana asociada a este ViewModel.
        /// </param>
        /// <param name="element">
        ///     <see cref="CrudElement"/> a incorporar como editor.
        /// </param>
        public CrudViewModel(ICloseable host, CrudElement element) : base(host)
        {
            Implementation = new DbBoundCrudViewModel(element);
            Implementation.ForwardChange(this);

            RegisterPropertyChangeTrigger(nameof(Title),
                nameof(EditMode));

            RegisterPropertyChangeBroadcast(nameof(EditMode), nameof(Closeable));
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="CrudViewModel{TService}"/>.
        /// </summary>
        /// <param name="host">Host visual de la ventana asociada a este ViewModel.</param>
        /// <param name="source">Origen de datos a utilizar.</param>
        /// <param name="models">Modelos asociados de datos.</param>
        public CrudViewModel(ICloseable host, IQueryable<ModelBase> source, params Type[] models) : base(host, true)
        {
            Implementation = new DbBoundCrudViewModel(source, models);
            Implementation.ForwardChange(this);
        }

        /// <summary>
        ///     Inicializa una nueva intstancia de la clase
        ///     <see cref="CrudViewModel{TService}"/>.
        /// </summary>
        /// <param name="host">Host visual de la ventana asociada a este ViewModel.</param>
        /// <param name="source">Origen de datos a utilizar.</param>
        /// <param name="elements">Elementos de crud a incorporar.</param>
        public CrudViewModel(ICloseable host, IQueryable<ModelBase> source, params CrudElement[] elements) : base(host, true)
        {
            Implementation = new DbBoundCrudViewModel(source, elements);
            Implementation.ForwardChange(this);
        }

        /// <summary>
        ///     Obtiene el título de esta ventana.
        /// </summary>
        public override string Title
        {
            get
            {
                if (EditMode)
                {
                    return ((Selection as ModelBase)?.IsNew ?? true) 
                        ? $"Nuevo {SelectedElement.Description.FriendlyName}"
                        : $"Editar {SelectedElement.Description.FriendlyName} {(Selection as ModelBase)?.StringId}";
                }
                return base.Title;
            }

            set => base.Title = value;
        }

        /// <summary>
        ///     Obtiene o establece un valor que indica si esta ventana puede
        ///     cerrarse.
        /// </summary>
        public override bool Closeable
        {
            get => base.Closeable && !EditMode;
            protected set => base.Closeable = value;
        }

        /// <summary>
        ///     Determina si es posible ejecutar el comando para la creación de
        ///     nuevas entidades.
        /// </summary>
        /// <param name="t">
        ///     Tipo de modelo.
        /// </param>
        /// <returns>
        ///     En su implementación predeterminada, este método siempre
        ///     devuelve <see langword="true"/>.
        /// </returns>
        public bool CanCreate(Type t) => ((ICrudCollectionViewModel)Implementation).CanCreate(t);

        /// <summary>
        ///     Determina si es posible editar a la entidad seleccionada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        ///     <see langword="true"/> si es posible editar la entidad
        ///     seleccionada, <see langword="false"/> en caso contrario.
        /// </returns>
        public bool CanEdit(ModelBase entity) => ((ICrudCollectionViewModel)Implementation).CanEdit(entity);

        /// <summary>
        ///     Determina si es posible eliminar a la entidad seleccionada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        ///     <see langword="true"/> si es posible eliminar la entidad
        ///     seleccionada, <see langword="false"/> en caso contrario.
        /// </returns>
        public bool CanDelete(ModelBase entity) => ((ICrudCollectionViewModel)Implementation).CanDelete(entity);

        /// <summary>
        ///     Ejecuta una operación colocando a este 
        ///     <see cref="ICrudEditingViewModel"/> en estado de ocupado.
        /// </summary>
        /// <param name="action">Tarea a ejecutar.</param>
        public void BusyDo(Task action) => Implementation.BusyDo(action);
    }
}