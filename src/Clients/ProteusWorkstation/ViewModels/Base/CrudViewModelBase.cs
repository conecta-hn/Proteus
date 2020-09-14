/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    /// Clase base para un ViewModel que implemente funcionalidad Crud.
    /// </summary>
    public abstract class CrudViewModelBase : CrudViewModelBasicBase, ICrudViewModel
    {
        internal static readonly IEnumerable<CrudTool> _allTools = Objects.FindAllObjects<CrudTool>();
        private bool _editMode;
        private Type? _selection;

        public double EditorWidth => Settings.Default.EditorWidth;

        /// <summary>
        /// Colección que describe las diferentes presentaciones
        /// disponibles de este <see cref="CrudViewModel{TService}"/>
        /// basado en el tipo de entidad seleccionada.
        /// </summary>
        protected ICollection<CrudElement> Elements { get; } = new List<CrudElement>();

        /// <summary>
        /// Enumera los modelos controlados por este <see cref="CrudViewModelBase"/>.
        /// </summary>
        public IEnumerable<Type> Models { get; }

        /// <summary>
        /// Obtiene o establece al elemento seleccionado.
        /// </summary>
        public ModelBase? Selection
        {
            get => SelectedElement?.ViewModel?.Entity as ModelBase;
            set
            {
                _selection = value?.GetType().ResolveToDefinedType();

                if (_selection is { } t)
                {
                    CheckElements(t ??= Models.First());
                }

                if (!PerformSelection(IsForType, value))
                    PerformSelection(Implements,value);

                OnPropertyChanged();

                // HACK: Bruteforce a notification ¯\_(ツ)_/¯
                if (SelectedElement is { } se)
                foreach (var j in se.EditControls)
                {
                    (se.ViewModel as INotifyPropertyChangeBase)?.Notify(j.Property.Name);
                }
            }
        }

        private bool PerformSelection(Func<CrudElement,bool> check, ModelBase? value)
        {
            var flag = false;
            foreach (var j in Elements.NotNull())
            {
                if (j.ViewModel is null) continue;
                if (check(j))
                {
                    flag = true;
                    if (!(j.ViewModel is null))
                    {
                        lock (value!) lock (j.ViewModel)
                                j.ViewModel.Entity = value;
                    }
                    foreach (var k in SelectedElement?.EditControls ?? Array.Empty<IPropertyMapping>())
                    {
                        try
                        {
                            k.GetValue(k.Description.PropertySource == PropertyLocation.Model ? (object)value! : SelectedElement!.ViewModel);
                        }
                        catch (TargetInvocationException)
                        {
                            try
                            {
                                k.GetValue(SelectedElement!.ViewModel);
                            }
                            catch (TargetInvocationException)
                            {
                                k.GetValue(value!);
                            }
                        }
                    }
                }
                else
                {
                    if (!(j.ViewModel is null)) j.ViewModel.Entity = null!;
                }
                j.ViewModel?.Refresh();
            }
            return flag || value is null;
        }

        /// <summary>
        /// Abstrae las comprobaciones complejas de tipos de modelos,
        /// tomando en cuenta si son modelos estáticos o Proxies compilados
        /// de Entity Framework.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool IsForType(CrudElement j) => IsForType(j, _selection);

        private bool IsForType(CrudElement j, Type? model)
        {
            return j.Model.ResolveToDefinedType() == model?.ResolveToDefinedType();
        }
        private bool Implements(CrudElement j) => Implements(j, _selection!);

        private bool Implements(CrudElement j, Type model)
        {
            return model.Implements(j.Model.ResolveToDefinedType()!);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CrudCollectionViewModelBase"/>.
        /// </summary>
        /// <param name="elements">
        /// Arreglo de <see cref="CrudElement"/> que serán utilizados para
        /// editar entidades dentro de esta instancia.
        /// </param>
        protected CrudViewModelBase(params Type[] elements)
        {
            if (!elements.Any()) throw new ArgumentException("Se necesita al menos un tipo administrado por este ViewModel.", new EmptyCollectionException(elements));

            Models = elements.Select(p => p.ResolveCollectionType()!.ResolveToDefinedType()!).ToList();

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
                    elements.Select(p => new Launcher(CrudElement.GetDescription(p)?.FriendlyName ?? p.Name, null, ((Action<object>)OnCreate).Method.FullName(),
                        new ObservingCommand(this, OnCreate, CanCreate, nameof(SelectedElement)), p)));
            }
            RegisterPropertyChangeBroadcast(nameof(IsBusy),
                nameof(BusyV), nameof(NotBusyV));
            RegisterPropertyChangeBroadcast(nameof(EditMode),
                nameof(NotEditMode), nameof(EditVis), nameof(NotEditVis));
            RegisterPropertyChangeBroadcast(nameof(Selection),
                nameof(SelectedEditor), nameof(SelectedElement), nameof(SelectedDetails));

            Settings.Default.PropertyChanged += Default_PropertyChanged;
        }

        private protected Type? ParentModel { get; set; }

        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Notify(e.PropertyName);
        }

        /// <summary>
        /// Obtiene el editor a utlizar para editar a la entidad seleccionada.
        /// </summary>
        public FrameworkElement? SelectedEditor => SelectedElement?.Editor;

        /// <summary>
        /// Obtiene la ventana de detalles de la entidad seleccionada.
        /// </summary>
        public FrameworkElement? SelectedDetails => SelectedElement?.Details;

        /// <summary>
        /// Obtiene un <see cref="CrudElement"/> con información sobre los
        /// componentes relacionados al modelo de datos de la entidad
        /// seleccionada.
        /// </summary>
        public override CrudElement SelectedElement
            => Elements.FirstOrDefault(IsForType)
            ?? Elements.FirstOrDefault(Implements);

        private protected void OnCancel()
        {
            if (Selection is ModelBase s)
            {
                if (NewMode || s.IsNew)
                {
                    Selection = null;
                }
                else
                {
                    Service!.Rollback(s);
                }
            }
            NewMode = false;
            EditMode = false;
            EnableEditCtrls();
        }

        private protected async Task OnSave()
        {
            if (Precheck()) return;
            var e = Selection;
            var r = await PerformSave(Selection!);
            if (r.Result == Result.Ok)
            {
                if (SelectedElement is null) Selection = e;
                await PostSave(e!);
                NewMode = false;
                EditMode = false;
            }
            else
            {
                Proteus.MessageTarget?.Error(r.Message);
            }
        }

        /// <summary>
        /// Implementa la operación de guardado según lo requerido por la
        /// clase base de este <see cref="CrudCollectionViewModelBase"/>.
        /// </summary>
        /// <param name="entity">
        /// Entidad a guardar.
        /// </param>
        /// <returns>
        /// UN objeto que representa el resultado de la operación.
        /// </returns>
        protected abstract Task<DetailedResult> PerformSave(ModelBase entity);

        /// <summary>
        /// Obtiene un valor que indica si el editor se ha abierto para
        /// crear una nueva entidad.
        /// </summary>
        protected bool NewMode { get; private set; } = false;

        /// <summary>
        /// Ejecuta una acción que Crea una nueva entidad.
        /// </summary>
        /// <param name="t">Modelo de datos a crear.</param>
        public void OnCreate(Type? t)
        {
            CheckElements(t ??= Models.First());
            NewMode = true;
            var entity = t.New<ModelBase>();
            Selection = entity;
            ClearCtrls(entity);
            OnEdit(entity);
        }

        public void CreateNewFrom(ModelBase entity)
        {
            CheckElements(entity.GetType());
            Selection = entity;
            NewMode = true;
            OnEdit(entity);
        }

        /// <summary>
        /// Determina si es posible ejecutar el comando para la creación de
        /// nuevas entidades.
        /// </summary>
        /// <param name="t">
        /// Tipo de modelo.
        /// </param>
        /// <returns>
        /// En su implementación predeterminada, este método siempre
        /// devuelve <see langword="true"/>.
        /// </returns>
        public virtual bool CanCreate(Type? t) => true;

        /// <summary>
        /// Determina si es posible editar a la entidad seleccionada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// <see langword="true"/> si es posible editar la entidad
        /// seleccionada, <see langword="false"/> en caso contrario.
        /// </returns>
        public virtual bool CanEdit(ModelBase? entity) => !(entity is null);

        /// <summary>
        /// Determina si es posible eliminar a la entidad seleccionada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// <see langword="true"/> si es posible eliminar la entidad
        /// seleccionada, <see langword="false"/> en caso contrario.
        /// </returns>
        public virtual bool CanDelete(ModelBase? entity) => !(entity is null) && (SelectedElement?.Description?.CanDelete?.Invoke(entity) ?? true);

        /// <summary>
        /// Comando para la creación de nuevas entidades.
        /// </summary>
        public ICommand CreateNew { get; }

        /// <summary>
        /// Comando para la edición de la entidad actualmente seleccionada.
        /// </summary>
        public ICommand EditCurrent { get; }
        /// <summary>
        /// Comando para la eliminación de la entidad actualmente
        /// seleccionada.
        /// </summary>
        public ICommand DeleteCurrent { get; }

        /// <summary>
        /// Comando de guardado de entidades, tanto nuevas como editadas.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Comando que cancela la creación o edición de una entidad.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Enumeración de comandos para la creación de entidades cuando
        /// este ViewModel administra dos o más modelos de datos.
        /// </summary>
        public IEnumerable<Launcher>? CreateCommands { get; }

        /// <summary>
        /// Ejecuta una operación de eliminación de información de la
        /// colección activa.
        /// </summary>
        /// <param name="o">
        /// Elemento a eliminar.
        /// </param>
        public abstract void OnDelete(object? o);

        /// <summary>
        /// Ejecuta una operación de edición de información de la
        /// colección activa.
        /// </summary>
        /// <param name="o">
        /// Elemento a eliminar.
        /// </param>
        public void OnEdit(object? o)
        {
            EditMode = true;

            if (NewMode || ((o as ModelBase)?.IsNew ?? true)) return;
            DisableIdCtrls();
        }

        [Sugar] internal void OnCreate(object? o) => OnCreate(o as Type);
        [Sugar] internal bool CanCreate(object? o) => CanCreate(o as Type);
        [Sugar] internal bool CanEdit(object? o) => CanEdit(Selection);
        [Sugar] internal bool CanDelete(object? o) => CanDelete(Selection);

        /// <summary>
        /// Ejecuta una operación colocando a este 
        /// <see cref="ICrudEditingViewModel"/> en estado de ocupado.
        /// </summary>
        /// <param name="action">Tarea a ejecutar.</param>
        public async void BusyDo(Task action)
        {
            IsBusy = true;
            await action;
            IsBusy = false;
        }

        /// <summary>
        /// Obtiene un valor de visibilidad aplicable mientras el ViewModel se encuentre ocupado.
        /// </summary>
        public Visibility BusyV => IsBusy ? Visibility.Visible : Visibility.Collapsed;
        /// <summary>
        /// Obtiene un valor de visibilidad aplicable mientras el ViewModel no se encuentre ovupado.
        /// </summary>
        public Visibility NotBusyV => !IsBusy ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Obtiene un valor de visibilidad aplicable cuando el ViewModel administre la creación de múltiples modelos.
        /// </summary>
        public Visibility MultiModel { get; }

        /// <summary>
        /// Obtiene un valor de visibilidad aplicacble cuando el ViewModel administre la creación de un único modelo.
        /// </summary>
        public Visibility UniModel { get; }

        /// <summary>
        /// Obtiene un valor que indica si el ViewModel se encuentra actualmente en modo de edición.
        /// </summary>
        public bool EditMode
        {
            get => _editMode;
            private set => Change(ref _editMode, value);
        }

        /// <summary>
        /// Obtiene un valor que indica si el ViewModel no se encuentra actualmente en modo de edición.
        /// </summary>
        public bool NotEditMode => !EditMode;

        /// <summary>
        /// Obtiene un valor que indica si el ViewModel no está ocupado.
        /// </summary>
        public bool NotBusy => !IsBusy;

        /// <summary>
        /// Obtiene un valor de visibilidad aplicable cuando el ViewModel se encuentre en modo de edición.
        /// </summary>
        public Visibility EditVis => EditMode ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Obtiene un valor de visibilidad aplicable cuando el ViewModel no se encuentre en modo de edición.
        /// </summary>
        public Visibility NotEditVis => NotEditMode ? Visibility.Visible : Visibility.Collapsed;

        private void CheckElements(Type t)
        {
            if (!Elements.Any(p => IsForType(p, t)) || !Elements.Any(p => Implements(p, t!)))
            {
                Elements.Add(new CrudElement(t, ParentModel));
            }
        }

        ~CrudViewModelBase()
        {
            Settings.Default.PropertyChanged -= Default_PropertyChanged;
        }
    }
}