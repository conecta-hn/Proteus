/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Widgets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using static TheXDS.MCART.Types.Extensions.MemberInfoExtensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.ViewModels.Base
{
    public abstract class CrudViewModelBase: ProteusViewModel, ICrudViewModel
    {
        /// <summary>
        ///     Obtiene una referencia al servicio a utilizar para las
        ///     operaciones CRUD de este ViewModel.
        /// </summary>
        protected Service Service => Proteus.Infer(SelectedElement?.Model);

        /// <summary>
        ///     Colección que describe las diferentes presentaciones
        ///     disponibles de este <see cref="CrudViewModel{TService}"/>
        ///     basado en el tipo de entidad seleccionada.
        /// </summary>
        protected IEnumerable<CrudElement> Elements { get; }

        /// <summary>
        ///     Obtiene o establece al elemento seleccionado.
        /// </summary>
        public object? Selection
        {
            get => SelectedElement?.ViewModel?.Entity;
            set
            {
                _selection = value?.GetType();
                foreach (var j in Elements.NotNull())
                {
                    if (j.ViewModel is null) continue;
                    if (IsForType(j))
                    {
                        if (!(j.ViewModel is null))
                        {
                            lock (value) lock (j.ViewModel)
                                    j.ViewModel.Entity = value;
                        }
                        foreach (var k in SelectedElement?.EditControls ?? Array.Empty<IPropertyMapping>())
                        {
                            k.GetValue(k.Description.PropertySource == PropertyLocation.Model ? value : SelectedElement.ViewModel);
                        }
                    }
                    else
                    {
                        if (!(j.ViewModel is null)) j.ViewModel.Entity = null;
                    }
                    j.ViewModel?.Refresh();
                }
                OnPropertyChanged();

                // HACK: Bruteforce a notification ¯\_(ツ)_/¯
                SelectedElement?.Commit();
            }
        }

        /// <summary>
        ///     Abstrae las comprobaciones complejas de tipos de modelos,
        ///     tomando en cuenta si son modelos estáticos o Proxies compilados
        ///     de Entity Framework.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool IsForType(CrudElement j)
        {
            return j.Model.ResolveToDefinedType() == _selection?.ResolveToDefinedType();
        }

        public CrudViewModelBase(params CrudElement[] elements)
        {
            if (!elements.Any()) throw new ArgumentException("Se necesita al menos un tipo administrado por este ViewModel.", new EmptyCollectionException(elements));
            Elements = elements.ToList();
            foreach (var j in Elements)
            {
                j.Description?.SetCurrentEditor(this);
            }
            CreateNew = new ObservingCommand(this, OnCreate, CanCreate, nameof(SelectedElement));
            EditCurrent = new ObservingCommand(this, OnEdit, CanEdit, nameof(SelectedElement));
            DeleteCurrent = new ObservingCommand(this, o => BusyOp(() => OnDelete(o)), CanDelete, nameof(SelectedElement));
            SaveCommand = new SimpleCommand(() => BusyOp(OnSave()), true);
            CancelCommand = new SimpleCommand(OnCancel, true);
            if (elements.Count() == 1)
            {
                MultiModel = Visibility.Collapsed;
                UniModel = Visibility.Visible;
            }
            else
            {
                MultiModel = Visibility.Visible;
                UniModel = Visibility.Collapsed;
                CreateCommands = new HashSet<Launcher>(
                    elements.Select(p => new Launcher(p.Description.FriendlyName, null, ((Action<object>)OnCreate).Method.FullName(),
                        new ObservingCommand(this, OnCreate, CanCreate, nameof(SelectedElement)), p.Model)));
            }
            RegisterPropertyChangeBroadcast(nameof(IsBusy),
                nameof(BusyV), nameof(NotBusyV));
            RegisterPropertyChangeBroadcast(nameof(EditMode),
                nameof(NotEditMode), nameof(EditVis), nameof(NotEditVis));
            RegisterPropertyChangeBroadcast(nameof(Selection),
                nameof(SelectedEditor), nameof(SelectedElement), nameof(SelectedDetails));
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="CrudCollectionViewModelBase"/>.
        /// </summary>
        /// <param name="models">Modelos asociados de datos.</param>
        public CrudViewModelBase(params Type[] models) : this(models.Select(p => new CrudElement(p)).ToArray())
        {
        }

        /// <summary>
        ///     Obtiene el editor a utlizar para editar a la entidad seleccionada.
        /// </summary>
        public FrameworkElement? SelectedEditor => SelectedElement?.Editor;

        /// <summary>
        ///     Obtiene la ventana de detalles de la entidad seleccionada.
        /// </summary>
        public FrameworkElement? SelectedDetails => SelectedElement?.Details;

        /// <summary>
        ///     Obtiene un <see cref="CrudElement"/> con información sobre los
        ///     componentes relacionados al modelo de datos de la entidad
        ///     seleccionada.
        /// </summary>
        public CrudElement SelectedElement => Elements.FirstOrDefault(IsForType);

        private bool _editMode;
        private Type _selection;

        private protected void OnCancel()
        {
            if (Selection is ModelBase s)
            {
                if (NewMode)
                {
                    Selection = null;
                }
                else
                {
                    Service.Rollback(s);
                }
            }
            NewMode = false;
            EditMode = false;
            foreach (var j in SelectedElement?.EditControls ?? Array.Empty<IPropertyMapping>()) j.ContainingControl.IsEnabled = true;
        }

        /// <summary>
        ///     Obtiene, de ser posible, a la entidad padre de la que se
        ///     encuentra actualmente seleccionada.
        /// </summary>
        /// <returns>
        ///     La entidad padre de la actualmente seleccionasa, o 
        ///     <see langword="null"/> si la entidad seleccionada no puede
        ///     tener información sobre su padre en este contexto.
        /// </returns>
        protected abstract ModelBase? GetParent();

        /// <summary>
        ///     Ejecuta una acción a realizar justo después de guardar la
        ///     información en la base de datos.
        /// </summary>
        protected abstract void AfterSave();

        private protected async Task OnSave()
        {
            var e = Selection as ModelBase;

            if (!(SelectedElement is null))
            {
                SelectedElement.Commit();

                var parent = GetParent();
                bool fail = false;

                try
                {
                    foreach (var j in SelectedElement.Description.BeforeSave)
                    {
                        j.CallSaves(e, parent);
                    }
                    if (SelectedElement.Description is IVmCrudDescription ivm)
                    {
                        foreach (var j in ivm.VmBeforeSave)
                        {
                            j.CallSaves(SelectedElement.ViewModel, parent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Proteus.MessageTarget?.Error(ex.Message);
                    fail = true;
                }

                foreach (var j in SelectedElement.Description.Descriptions)
                {
                    var f = j.Validator?.Invoke(e, j.Property)?.ToList();
                    if (f?.Any() ?? false)
                    {
                        fail = true;
                        Proteus.MessageTarget?.Stop(string.Concat(f.Select(p => $"{p.FailedProperty?.NameOf().OrNull("{0}: ")}{p.Message}\n")));
                    }
                }
                if (fail) return;
            }

            if (Settings.Default.CheckExists)
            {
                if (Service.Exists(e))
                {
                    Proteus.MessageTarget?.Stop($"Ya existe un elemento con el Id '{e.StringId}' en la base de datos.");
                    return;
                }
            }

            var r = await PerformSave(e);
            if (SelectedElement is null)
            {
                Selection = e;
            }
            try
            {
                foreach (var j in SelectedElement.Description.AfterSave)
                {
                    j.CallSaves(e, null);
                }
                if (SelectedElement?.Description is IVmCrudDescription ivm)
                {
                    foreach (var j in ivm.VmBeforeSave)
                    {
                        j.CallSaves(SelectedElement.ViewModel, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Critical(ex);
            }

            AfterSave();
            NewMode = false;
            EditMode = false;
            SelectedElement?.ViewModel.Refresh();
            foreach (var j in SelectedElement?.EditControls ?? Array.Empty<IPropertyMapping>()) j.ContainingControl.IsEnabled = true;
        }

        /// <summary>
        ///     Implementa la operación de guardado según lo requerido por la
        ///     clase base de este <see cref="CrudCollectionViewModelBase"/>.
        /// </summary>
        /// <param name="entity">
        ///     Entidad a guardar.
        /// </param>
        /// <returns>
        ///     UN objeto que representa el resultado de la operación.
        /// </returns>
        protected abstract Task<DetailedResult> PerformSave(ModelBase entity);

        /// <summary>
        ///     Obtiene un valor que indica si el editor se ha abierto para
        ///     crear una nueva entidad.
        /// </summary>
        protected bool NewMode { get; private set; } = false;
        private void OnCreate(Type t)
        {
            NewMode = true;
            var entity = (t ?? Elements.FirstOrDefault()?.Model).New();
            Selection = entity;
            foreach (var k in SelectedElement?.EditControls ?? Array.Empty<IPropertyMapping>())
            {
                if (k.Description.UseDefault && k.Property.CanWrite)
                {
                    k.Property.SetValue(entity, k.Description.Default);
                    (SelectedElement.ViewModel as NotifyPropertyChangeBase)?.Notify(k.Property.Name);
                    k.GetValue(entity);
                }
                else k.ClearControlValue();
            }
            OnEdit(entity);
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
        public virtual bool CanCreate(Type t) => true;

        /// <summary>
        ///     Determina si es posible editar a la entidad seleccionada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        ///     <see langword="true"/> si es posible editar la entidad
        ///     seleccionada, <see langword="false"/> en caso contrario.
        /// </returns>
        public virtual bool CanEdit(ModelBase entity) => !(entity is null);

        /// <summary>
        ///     Determina si es posible eliminar a la entidad seleccionada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        ///     <see langword="true"/> si es posible eliminar la entidad
        ///     seleccionada, <see langword="false"/> en caso contrario.
        /// </returns>
        public virtual bool CanDelete(ModelBase entity) => !(entity is null) && (SelectedElement?.Description?.CanDelete?.Invoke(entity) ?? true);

        /// <summary>
        ///     Comando para la creación de nuevas entidades.
        /// </summary>
        public ICommand CreateNew { get; }

        /// <summary>
        ///     Comando para la edición de la entidad actualmente seleccionada.
        /// </summary>
        public ICommand EditCurrent { get; }
        /// <summary>
        ///     Comando para la eliminación de la entidad actualmente
        ///     seleccionada.
        /// </summary>
        public ICommand DeleteCurrent { get; }

        /// <summary>
        ///     Comando de guardado de entidades, tanto nuevas como editadas.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        ///     Comando que cancela la creación o edición de una entidad.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        ///     Enumeración de comandos para la creación de entidades cuando
        ///     este ViewModel administra dos o más modelos de datos.
        /// </summary>
        public IEnumerable<Launcher> CreateCommands { get; }

        /// <summary>
        ///     Ejecuta una operación de eliminado de información de la
        ///     colección activa.
        /// </summary>
        /// <param name="o">
        ///     Elemento a eliminar.
        /// </param>
        protected abstract void OnDelete(object o);

        private protected void OnEdit(object o)
        {
            EditMode = true;

            if (NewMode || ((o as ModelBase)?.IsNew ?? true)) return;
            foreach (var j in SelectedElement?.EditControls ?? Array.Empty<IPropertyMapping>())
            {
                if (j.Property.HasAttr<KeyAttribute>())
                {
                    j.ContainingControl.IsEnabled = false;
                }
            }
        }

        [Sugar] private void OnCreate(object o) => OnCreate(o as Type);
        [Sugar] private bool CanCreate(object o) => CanCreate(o as Type);
        [Sugar] private bool CanEdit(object o) => CanEdit((ModelBase)Selection);
        [Sugar] private bool CanDelete(object o) => CanDelete((ModelBase)Selection);

        /// <summary>
        ///     Ejecuta una operación colocando a este 
        ///     <see cref="ICrudEditingViewModel"/> en estado de ocupado.
        /// </summary>
        /// <param name="action">Tarea a ejecutar.</param>
        public async void BusyDo(Task action)
        {
            IsBusy = true;
            await action;
            IsBusy = false;
        }

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable mientras el ViewModel se encuentre ocupado.
        /// </summary>
        public Visibility BusyV => IsBusy ? Visibility.Visible : Visibility.Collapsed;
        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable mientras el ViewModel no se encuentre ovupado.
        /// </summary>
        public Visibility NotBusyV => !IsBusy ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable cuando el ViewModel administre la creación de múltiples modelos.
        /// </summary>
        public Visibility MultiModel { get; }

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicacble cuando el ViewModel administre la creación de un único modelo.
        /// </summary>
        public Visibility UniModel { get; }

        /// <summary>
        ///     Obtiene un valor que indica si el ViewModel se encuentra actualmente en modo de edición.
        /// </summary>
        public bool EditMode
        {
            get => _editMode;
            private set => Change(ref _editMode, value);
        }

        /// <summary>
        ///     Obtiene un valor que indica si el ViewModel no se encuentra actualmente en modo de edición.
        /// </summary>
        public bool NotEditMode => !EditMode;

        /// <summary>
        ///     Obtiene un valor que indica si el ViewModel no está ocupado.
        /// </summary>
        public bool NotBusy => !IsBusy;

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable cuando el ViewModel se encuentre en modo de edición.
        /// </summary>
        public Visibility EditVis => EditMode ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        ///     Obtiene un valor de visibilidad aplicable cuando el ViewModel no se encuentre en modo de edición.
        /// </summary>
        public Visibility NotEditVis => NotEditMode ? Visibility.Visible : Visibility.Collapsed;
    }
}