/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using static TheXDS.MCART.ReflectionHelpers;
using static TheXDS.MCART.Types.Extensions.StringExtensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;
using System.Threading.Tasks;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Clase base para describir la presentación y generación de ventanas
    /// Crud para un modelo de datos que incluye definiciones de ViewModel.
    /// </summary>
    /// <typeparam name="TModel">
    /// Tipo de modelo a describir.
    /// </typeparam>
    /// <typeparam name="TViewModel">
    /// Tipo de ViewModel a describir.
    /// </typeparam>
    public abstract class CrudDescriptor<TModel, TViewModel> : CrudDescriptor<TModel>, IVmCrudDescription where TModel : ModelBase, new() where TViewModel : class, IEntityViewModel<TModel>, new()
    {
        private class SaveActionChain : IVmSaveActionChain<TModel, TViewModel>
        {
            private readonly HashSet<Action<TViewModel, ModelBase>> _actions = new HashSet<Action<TViewModel, ModelBase>>();

            public SaveActionChain(Action<TViewModel, ModelBase> action)
            {
                Then(action);
            }
            public SaveActionChain(Action<TViewModel> action)
            {
                Then(action);
            }

            public void CallSaves(object obj, ModelBase parent)
            {
                foreach (var j in _actions)
                    j?.Invoke(obj as TViewModel ?? throw new InvalidCastException(), parent);
            }

            public IVmSaveActionChain<TModel, TViewModel> Then(Action<TViewModel> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                return Then((m, _) => action(m));
            }

            public IVmSaveActionChain<TModel, TViewModel> Then(Action<TViewModel, ModelBase> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                _actions.Add(action);
                return this;
            }
        }

        private readonly HashSet<SaveActionChain> _beforeSave = new HashSet<SaveActionChain>();
        private readonly HashSet<SaveActionChain> _afterSave = new HashSet<SaveActionChain>();

        /// <summary>
        /// Obtiene una referencia a una propiedad dentro de un ViewModel.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyDescriptor"/> por medio del cual se
        /// puede configurar la presentación de la propiedad en una ventana
        /// de Crud.
        /// </returns>
        protected IPropertyDescriptor VmProperty(Expression<Func<TViewModel, object?>> propertySelector)
        {
            return Prop<CrudPropertyDescriptor, object?>(propertySelector);
        }

        /// <summary>
        /// Obtiene una referencia a una propiedad numérica dentro de un
        /// ViewModel.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyNumberDescriptor{TValue}"/> por medio
        /// del cual se puede configurar la presentación de la propiedad en
        /// una ventana de Crud.
        /// </returns>
        protected IPropertyNumberDescriptor<TValue> VmNumericProperty<TValue>(Expression<Func<TViewModel, TValue>> propertySelector) where TValue : IComparable<TValue>
        {
            return Prop<CrudNumericPropertyDescriptor<TValue>, TValue>(propertySelector);
        }

        /// <summary>
        /// Obtiene una referencia a una propiedad de texto dentro de un
        /// ViewModel.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyTextDescriptor"/> por medio
        /// del cual se puede configurar la presentación de la propiedad en
        /// una ventana de Crud.
        /// </returns>
        protected IPropertyTextDescriptor VmTextProperty(Expression<Func<TViewModel, string?>> propertySelector)
        {
            return Prop<CrudTextPropertyDescriptor, string?>(propertySelector);
        }

        /// <summary>
        /// Obtiene una referencia a una propiedad de fecha dentro de un
        /// ViewModel.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyDateDescriptor"/> por medio
        /// del cual se puede configurar la presentación de la propiedad en
        /// una ventana de Crud.
        /// </returns>
        protected IPropertyDateDescriptor VmDateProperty(Expression<Func<TViewModel, DateTime>> propertySelector)
        {
            var r = Prop<DatePropertyDescriptor, DateTime>(propertySelector);
            r.PropertySource = PropertyLocation.ViewModel;
            return r;
        }

        /// <summary>
        /// Obtiene un <see cref="IListPropertyDescriptor{T}"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IListPropertyDescriptor{T}"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IListPropertyDescriptor<TItem> VmListProperty<TItem>(Expression<Func<TViewModel, IEnumerable<TItem>>> propertySelector) where TItem : ModelBase
        {
            var r = Prop<ListPropertyDescriptor<TItem>, IEnumerable<TItem>, TViewModel>(propertySelector);
            r.PropertySource = PropertyLocation.ViewModel;
            return r;
        }

        /// <summary>
        /// Obtiene un <see cref="ILinkPropertyDescriptor{T}"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="ILinkPropertyDescriptor{T}"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public ILinkPropertyDescriptor<TLink> VmLinkProperty<TLink>(Expression<Func<TViewModel, object?>> propertySelector) where TLink : ModelBase
        {
            var r = Prop<LinkPropertyDescriptor<TLink>, object?, TViewModel>(propertySelector);
            r.PropertySource = PropertyLocation.ViewModel;
            return r;
        }

        /// <summary>
        /// Obtiene un <see cref="IObjectPropertyDescriptor"/> con el cual
        /// se puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IObjectPropertyDescriptor"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IObjectPropertyDescriptor VmObjectProperty(Expression<Func<TViewModel, ModelBase?>> propertySelector)
        {
            var r = Prop<ObjectPropertyDescriptor, ModelBase?, TViewModel>(propertySelector);
            r.PropertySource = PropertyLocation.ViewModel;
            return r;
        }


        /// <summary>
        /// Obtiene el tipo efectivo de ViewModel a utilizar como base para
        /// compilar.
        /// </summary>
        public override Type BaseViewModelType => typeof(TViewModel);

        IEnumerable<ICallSaveAction> IVmCrudDescription.VmBeforeSave => _beforeSave;
        IEnumerable<ICallSaveAction> IVmCrudDescription.VmAfterSave => _afterSave;

        /// <summary>
        /// Define una acción a ejecutar previamente a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        protected IVmSaveActionChain<TModel, TViewModel> VmBeforeSave(Action<TViewModel, ModelBase> action)
        {
            return _beforeSave.Push(new SaveActionChain(action));
        }

        /// <summary>
        /// Define una acción a ejecutar previamente a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        protected IVmSaveActionChain<TModel, TViewModel> VmBeforeSave(Action<TViewModel> action)
        {
            return _beforeSave.Push(new SaveActionChain(action));
        }

        /// <summary>
        /// Define una acción a ejecutar previamente a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        protected IVmSaveActionChain<TModel, TViewModel> VmAfterSave(Action<TViewModel, ModelBase> action)
        {
            return _afterSave.Push(new SaveActionChain(action));
        }

        /// <summary>
        /// Define una acción a ejecutar previamente a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        protected IVmSaveActionChain<TModel, TViewModel> VmAfterSave(Action<TViewModel> action)
        {
            return _afterSave.Push(new SaveActionChain(action));
        }

        private TDescriptor Prop<TDescriptor, TProperty>(Expression<Func<TViewModel, TProperty>> propertySelector) where TDescriptor : IPropertyDescriptor, IPropertyDescription
        {
            var r = Prop<TDescriptor, TProperty, TViewModel>(propertySelector);
            if (r is CrudPropertyDescriptor cpd) cpd.PropertySource = PropertyLocation.ViewModel;
            return r;
        }
    }

    /// <summary>
    /// Clase base para describir la presentación y generación de ventanas
    /// Crud para un modelo de datos.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de modelo a describir.
    /// </typeparam>
    public abstract class CrudDescriptor<T> : ICrudDescription where T : ModelBase, new()
    {
        private class SaveActionChain : ISaveActionChain<T>
        {
            private readonly HashSet<Action<T, ModelBase?>> _actions = new HashSet<Action<T, ModelBase?>>();

            public SaveActionChain(Action<T, ModelBase> action)
            {
                Then(action);
            }
            public SaveActionChain(Action<T> action)
            {
                Then(action);
            }

            public void CallSaves(object obj, ModelBase? parent)
            {
                foreach (var j in _actions)
                    j?.Invoke(obj as T ?? throw new InvalidCastException(), parent);
            }

            public ISaveActionChain<T> Then(Action<T> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                return Then((m, _) => action(m));
            }

            public ISaveActionChain<T> Then(Action<T, ModelBase> action)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));

                _actions.Add(action);
                return this;
            }
        }

        private readonly HashSet<SaveActionChain> _beforeSave = new HashSet<SaveActionChain>();
        private readonly HashSet<SaveActionChain> _afterSave = new HashSet<SaveActionChain>();
        private Func<ModelBase, bool>? _canCreate;
        private Func<ModelBase, bool>? _canEdit;
        private Func<ModelBase, bool>? _canDelete;
        private string? _friendlyName;
        private Control? _details;
        private DataTemplate? _trvTemplate;
        private InteractionType? _onModuleMenu;
        private Func<bool>? _preCondition;
        private protected readonly HashSet<IPropertyDescription> _properties = new HashSet<IPropertyDescription>();
        private protected readonly Dictionary<string, Action<ModelBase, NotifyPropertyChangeBase>> _customActions = new Dictionary<string, Action<ModelBase, NotifyPropertyChangeBase>>();
        private readonly HashSet<Column> _listColumns = new HashSet<Column>();

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CrudDescriptor{T}"/>.
        /// </summary>
        protected CrudDescriptor()
        {
            DescribeModel();
        }

        /// <summary>
        /// Ejecuta las tareas de descripción de un modelo de datos.
        /// </summary>
        protected abstract void DescribeModel();

        /// <summary>
        /// Obtiene una referencia al editor actualmente activo.
        /// </summary>
        protected ICrudEditingViewModel? CurrentEditor { get; private set; }

        void ICrudDescription.SetCurrentEditor(ICrudEditingViewModel vm) => CurrentEditor = vm;

        #region ICrudDescription

        /// <summary>
        /// Enumera a las descripciones definidas para las propiedades del
        /// modelo.
        /// </summary>
        IEnumerable<IPropertyDescription> ICrudDescription.Descriptions => _properties;

        /// <summary>
        /// Obtiene una función definida a ejecutar previo a guardar una
        /// enitdad.
        /// </summary>
        IEnumerable<ICallSaveAction> ICrudDescription.BeforeSave => _beforeSave;
        IEnumerable<ICallSaveAction> ICrudDescription.AfterSave => _afterSave;

        Func<ModelBase, bool>? ICrudDescription.CanCreate => _canCreate;

        Func<ModelBase, bool>? ICrudDescription.CanEdit => _canEdit;

        Func<ModelBase, bool>? ICrudDescription.CanDelete => _canDelete;

        /// <summary>
        /// Enumera una serie de acciones personalizadas definidas para 
        /// mostrar en la venana del editor de Crud.
        /// </summary>
        IDictionary<string, Action<ModelBase, NotifyPropertyChangeBase>> ICrudDescription.CustomActions => _customActions;

        /// <summary>
        ///  Enumera las columnas a mostrar cuando el modelo sea presentado en 
        ///  un control <see cref="ListView"/>.
        /// </summary>
        IEnumerable<Column> ICrudDescription.ListColumns
        {
            get
            {
                var c = _listColumns.Concat(_properties
                    .Where(p => p.IsListColumn)
                    .Select(p => new Column(p.Label, p.Property.Name) { Format = p.ReadOnlyFormat }));

                if (c.Any()) return c;

                return ((ICrudDescription)this).DescribedModel.Implements<INameable>()
                    ? (new[] { new Column("Nombre descriptivo", "Name") })
                    : (new[] { new Column("Id", "Id") });
            }
        }

        /// <summary>
        /// Obtiene el nombre amigable establecido para este modelo.
        /// </summary>
        string ICrudDescription.FriendlyName => _friendlyName.OrNull() ?? typeof(T).Name;

        /// <summary>
        /// Obtiene un nombre amigable a utilizar para nomrbar al modelo.
        /// </summary>
        public virtual Type BaseViewModelType => null;

        /// <summary>
        /// Obtiene una referencia al tipo de modelo descrito.
        /// </summary>
        Type ICrudDescription.DescribedModel => typeof(T);

        /// <summary>
        /// Obtiene una página personalizada de detalles.
        /// </summary>
        Control ICrudDescription.Details => _details;

        DataTemplate? ICrudDescription.TreeViewTemplate => _trvTemplate;

        InteractionType? ICrudDescription.OnModuleMenu => _onModuleMenu;

        Func<bool>? ICrudDescription.LaunchPreCondition => _preCondition;

        #endregion

        #region Métodos de descripción

        /// <summary>
        /// Configura un modelo para aparecer de forma automática en el 
        /// menú del módulo.
        /// </summary>
        /// <param name="type">
        /// Tipo de interacción.
        /// </param>
        [DebuggerStepThrough]
        public void OnModuleMenu(InteractionType type)
        {
            OnModuleMenu(type, () => true);
        }

        [DebuggerStepThrough]
        public void OnModuleMenu(InteractionType type, Func<bool> preCondition)
        {
            if (_onModuleMenu.HasValue) throw new InvalidOperationException("La entrada autogenerada de menú ya ha sido configurada.");
            _onModuleMenu = type;
            _preCondition = preCondition;
        }

        /// <summary>
        /// Configura un modelo para aparecer de forma automática en el 
        /// menú del módulo.
        /// </summary>
        [DebuggerStepThrough]
        public void OnModuleMenu()
        {
            OnModuleMenu(InteractionType.AdminTool);
        }

        /// <summary>
        /// Registra una plantilla para utilizar al mostrar entidades de
        /// este modelo en un control <see cref="TreeView"/>.
        /// </summary>
        /// <param name="templateName">
        /// Nombre de la plantilla a utilizar.
        /// </param>
        public void Template(string templateName)
        {
            _trvTemplate = App.UiInvoke(() => Application.Current.TryFindResource(templateName) as DataTemplate);
        }

        /// <summary>
        /// Registra una plantilla para utilizar al mostrar entidades de
        /// este modelo en un control <see cref="TreeView"/>.
        /// </summary>
        public void Template()
        {
            Template($"{typeof(T).ResolveToDefinedType()!.Name}Template");
        }

        /// <summary>
        /// Muesra todos los campos descritos hasta ahora en la vista
        /// autogenerada de detalles.
        /// </summary>
        public void ShowAllInDetails()
        {
            foreach (var j in _properties)
            {
                (j as IPropertyDescriptor)?.ShowInDetails();
            }
        }

        /// <summary>
        /// Muestra todos los campos descritos hasta ahora como columnas 
        /// de lista.
        /// </summary>
        public void AllListColumn()
        {
            foreach (var j in _properties)
            {
                if (j.PropertyType != typeof(string) && j.PropertyType.Implements<IEnumerable>()) continue;
                (j as IPropertyDescriptor)?.AsListColumn();
            }
        }

        /// <summary>
        /// Establece una página personalizada de detalles a utilizar.
        /// </summary>
        /// <typeparam name="TDetails"></typeparam>
        public void CustomDetails<TDetails>() where TDetails : Control, new()
        {
            if (!(_details is null)) throw new InvalidOperationException("Ya se ha establecido una página personalizada de detalles.");
            _details = new TDetails();
        }

        /// <summary>
        /// Define una acción a ejecutar previamente a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        protected ISaveActionChain<T> BeforeSave(Action<T, ModelBase> action)
        {
            return _beforeSave.Push(new SaveActionChain(action));
        }

        /// <summary>
        /// Define una acción a ejecutar previamente a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> BeforeSave(Action<T> action)
        {
            return BeforeSave((m, _) => action?.Invoke(m));
        }

        /// <summary>
        /// Define una acción a ejecutar previamente a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> BeforeSave(Action action)
        {
            return BeforeSave((_, m) => action?.Invoke());
        }

        /// <summary>
        /// Define una acción a ejecutar previamente a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        protected ISaveActionChain<T> BeforeSave<TParent>(Action<T, TParent?> action) where TParent : ModelBase
        {
            return BeforeSave((m, p) => action?.Invoke(m, p as TParent));
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="task">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> BeforeSave(Func<Task> task)
        {
            return BeforeSave(async (_, m) => await task.Invoke());
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="task">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> BeforeSave(Func<T, Task> task)
        {
            return BeforeSave(async (m, _) => await task.Invoke(m));
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="task">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> BeforeSave<TParent>(Func<T, TParent?, Task> task) where TParent : ModelBase
        {
            return BeforeSave(async (m, p) => await task.Invoke(m, p as TParent));
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="task">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> BeforeSave(Func<T, ModelBase?, Task> task)
        {
            return BeforeSave(async (m, p) => await task.Invoke(m, p as ModelBase));
        }

        /// <summary>
        /// Define una acción a ejecutar luego de guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        protected ISaveActionChain<T> AfterSave(Action<T, ModelBase> action)
        {
            return _afterSave.Push(new SaveActionChain(action));
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> AfterSave(Action<T> action)
        {
            return AfterSave((m, _) => action?.Invoke(m));
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> AfterSave(Action action)
        {
            return AfterSave((_, m) => action?.Invoke());
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="task">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> AfterSave(Func<Task> task)
        {
            return AfterSave(async (_, m) => await task.Invoke());
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="task">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> AfterSave(Func<T, Task> task)
        {
            return AfterSave(async (m, _) => await task.Invoke(m));
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="task">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> AfterSave<TParent>(Func<T, TParent?, Task> task) where TParent : ModelBase
        {
            return AfterSave(async (m, p) => await task.Invoke(m, p as TParent));
        }

        /// <summary>
        /// Define una acción a ejecutar luego de a guardar una entidad.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        public ISaveActionChain<T> AfterSave<TParent>(Action<T, TParent?> action) where TParent : ModelBase
        {
            return AfterSave((m, p) => action?.Invoke(m, p as TParent));
        }

        /// <summary>
        /// Establece un método de comprobación de creación de entidades.
        /// </summary>
        /// <param name="check"></param>
        public void CanCreate(Func<ModelBase, bool> check)
        {
            if (!(_canCreate is null)) throw new InvalidOperationException("Ya se ha configurado una comprobación para esta acción.");
            _canCreate = check;
        }

        /// <summary>
        /// Establece un método de comprobación de edición de entidades.
        /// </summary>
        /// <param name="editCheck"></param>
        public void CanEdit(Func<T, bool> editCheck)
        {
            SetCanAction(ref _canEdit, editCheck);
        }    
        
        /// <summary>
        /// Establece un método de comprobación de borrado de entidades.
        /// </summary>
        /// <param name="deletionCheck"></param>
        public void CanDelete(Func<T, bool> deletionCheck)
        {
            SetCanAction(ref _canDelete, deletionCheck);
        }

        /// <summary>
        /// Configura el descriptor para establecer un valor duro que
        /// determina la posibilidad de crear entidades.
        /// </summary>
        /// <param name="value">
        /// valor a establecer.
        /// </param>
        public void CanCreate(bool value)
        {
            CanCreate(_ => value);
        }

        /// <summary>
        /// Configura el descriptor para establecer un valor duro que
        /// determina la posibilidad de editar entidades.
        /// </summary>
        /// <param name="value">
        /// valor a establecer.
        /// </param>
        public void CanEdit(bool value)
        {
            CanEdit(_ => value);
        }

        /// <summary>
        /// Configura el descriptor para establecer un valor duro que
        /// determina la posibilidad de eliminar entidades.
        /// </summary>
        /// <param name="value">
        /// valor a establecer.
        /// </param>
        public void CanDelete(bool value)
        {
            CanDelete(_ => value);
        }

        private void SetCanAction(ref Func<ModelBase, bool>? action, Func<T, bool> check)
        {
            //if (!(action is null)) throw new InvalidOperationException("Ya se ha configurado una comprobación para esta acción.");
            action = m => check(m as T ?? throw new InvalidCastException());
        }

        /// <summary>
        /// Obtiene un <see cref="IPropertyDescriptor"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyDescriptor"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IPropertyDescriptor Property(Expression<Func<T, object?>> propertySelector)
        {
            return Prop<CrudPropertyDescriptor, object?, T>(propertySelector);
        }

        /// <summary>
        /// Obtiene un <see cref="IPropertyNumberDescriptor{T}"/> con el
        /// cual se puede configurar la presentación de una propiedad en
        /// una ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyNumberDescriptor{T}"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IPropertyNumberDescriptor<TValue> NumericProperty<TValue>(Expression<Func<T, TValue>> propertySelector) where TValue : IComparable<TValue>
        {
            return Prop<CrudNumericPropertyDescriptor<TValue>, TValue, T>(propertySelector);
        }

        /// <summary>
        /// Obtiene un <see cref="IPropertyNumberDescriptor{T}"/> con el
        /// cual se puede configurar la presentación de una propiedad en
        /// una ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyNumberDescriptor{T}"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IPropertyNumberDescriptor<TValue> NumericProperty<TValue>(Expression<Func<T, TValue?>> propertySelector) where TValue : struct, IComparable<TValue>
        {
            return Prop<CrudNumericPropertyDescriptor<TValue>, TValue, T>(propertySelector);
        }

        /// <summary>
        /// Obtiene un <see cref="IPropertyTextDescriptor"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyTextDescriptor"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IPropertyTextDescriptor TextProperty(Expression<Func<T, string?>> propertySelector)
        {
            return Prop<CrudTextPropertyDescriptor, string?, T>(propertySelector);
        }

        /// <summary>
        /// Obtiene un <see cref="IPropertyDateDescriptor"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyDateDescriptor"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IPropertyDateDescriptor DateProperty(Expression<Func<T, DateTime>> propertySelector)
        {
            var p = Prop<DatePropertyDescriptor, DateTime, T>(propertySelector);
            return p;
        }

        /// <summary>
        /// Obtiene un <see cref="IPropertyDateDescriptor"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IPropertyDateDescriptor"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IPropertyDateDescriptor DateProperty(Expression<Func<T, DateTime?>> propertySelector)
        {
            var p = Prop<DatePropertyDescriptor, DateTime, T>(propertySelector);
            return p;
        }

        /// <summary>
        /// Obtiene un <see cref="IObjectPropertyDescriptor"/> con el cual
        /// se puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IObjectPropertyDescriptor"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IObjectPropertyDescriptor ObjectProperty(Expression<Func<T, ModelBase?>> propertySelector)
        {
            return Prop<ObjectPropertyDescriptor, ModelBase?, T>(propertySelector);
        }

        /// <summary>
        /// Obtiene un <see cref="IListPropertyDescriptor{T}"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IListPropertyDescriptor{T}"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public IListPropertyDescriptor<TItem> ListProperty<TItem>(Expression<Func<T, IEnumerable<TItem>>> propertySelector) where TItem : ModelBase
        {
            return Prop<ListPropertyDescriptor<TItem>, IEnumerable<TItem>, T>(propertySelector);
        }

        /// <summary>
        /// Obtiene un <see cref="ILinkPropertyDescriptor{T}"/> con el cual se
        /// puede configurar la presentación de una propiedad en una
        /// ventana autogenerada de Crud utilizando sintáxis Fluent.
        /// </summary>
        /// <param name="propertySelector">
        /// Expresión de selección de propiedad. Debe ser un acceso simple
        /// a un valor del modelo de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="ILinkPropertyDescriptor{T}"/> con el cual se puede
        /// configurar la presentación de una propiedad en una ventana
        /// autogenerada de Crud utilizando sintáxis Fluent.
        /// </returns>
        [DebuggerStepThrough]
        public ILinkPropertyDescriptor<TLink> LinkProperty<TLink>(Expression<Func<T, object>> propertySelector) where TLink : ModelBase
        {
            return Prop<LinkPropertyDescriptor<TLink>, object, T>(propertySelector);
        }

        /// <summary>
        /// Agrega una acción personalizada a la ventana del editor de Crud
        /// autogenerada.
        /// </summary>
        /// <param name="label">
        /// Etiqueta del botón
        /// </param>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        public void CustomAction(string label, Action<T> action)
        {
            if (label.IsEmpty()) throw new ArgumentNullException(nameof(label));
            if (action is null) throw new ArgumentNullException(nameof(action));
            _customActions.Add(label, (m ,_) => action?.Invoke(m as T ?? throw new InvalidCastException()));
        }

        /// <summary>
        /// Agrega una acción personalizada a la ventana del editor de Crud
        /// autogenerada.
        /// </summary>
        /// <param name="label">
        /// Etiqueta del botón
        /// </param>
        /// <param name="action">
        /// Acción a ejecutar.
        /// </param>
        public void CustomAction(string label, Action<T, NotifyPropertyChangeBase> action)
        {
            if (label.IsEmpty()) throw new ArgumentNullException(nameof(label));
            if (action is null) throw new ArgumentNullException(nameof(action));
            _customActions.Add(label, (m, n) => action?.Invoke(m as T ?? throw new InvalidCastException(), n));
        }

        /// <summary>
        /// Define las columnas a mostrar en los controles 
        /// <see cref="ListView"/>.
        /// </summary>
        /// <param name="columns">
        /// Columnas a mostrar.
        /// </param>
        public void ListColumns(params Column[] columns)
        {
            ListColumns(columns.AsEnumerable());
        }

        /// <summary>
        /// Define las columnas a mostrar en los controles 
        /// <see cref="ListView"/>.
        /// </summary>
        /// <param name="columns">
        /// Columnas a mostrar.
        /// </param>
        public void ListColumns(IEnumerable<Column> columns)
        {
            foreach (var j in columns) ListColumn(j);
        }

        /// <summary>
        /// Agrega una nueva columna de lista a presentar en un control
        /// <see cref="ListView"/>.
        /// </summary>
        public void ListColumn(Column c)
        {
            _listColumns.Add(c);
        }

        /// <summary>
        /// Establece un nombre amigable a utilizar para nombrar al modelo.
        /// </summary>
        /// <param name="name">Nombre amigable del modelo.</param>
        public void FriendlyName(string name)
        {
            _friendlyName = name;
        }

        /// <summary>
        /// Registra una función personalizada de presentación de una
        /// entidad al mostrarse en un contexto de solo lectura.
        /// </summary>
        /// <param name="presenter">
        /// Función que realizará la conversión.
        /// </param>
        public void RegisterReadOnlyPresenter(Func<T, string> presenter)
        {
            ReadOnlyPresenter._customConversions.Add(typeof(T), m => presenter((T)m));
        }

        #endregion

        [DebuggerStepThrough]
        private protected TDescriptor Prop<TDescriptor, TProperty, TObject>(Expression<Func<TObject, TProperty>> propertySelector) where TDescriptor : IPropertyDescriptor, IPropertyDescription
        {
            var p = GetMember(propertySelector) as PropertyInfo
                ?? throw new MemberAccessException("El miembro a seleccionar debe ser una propiedad.");

            if (_properties.Any(q => q.Property == p))
                //throw new InvalidOperationException($"La propiedad '{p.Name}' ya ha sido configurada.");
                return _properties.First(q => q.Property == p) is TDescriptor td ? td : default;

            var cpd = typeof(TDescriptor).New<TDescriptor>(p);
            _properties.Add(cpd);
            return cpd;
        }
        [DebuggerStepThrough]
        private protected TDescriptor Prop<TDescriptor, TProperty, TObject>(Expression<Func<TObject, TProperty?>> propertySelector) where TDescriptor : IPropertyDescriptor, IPropertyDescription where TProperty : struct
        {
            var p = GetMember(propertySelector) as PropertyInfo
                ?? throw new MemberAccessException("El miembro a seleccionar debe ser una propiedad.");

            if (_properties.Any(q => q.Property == p))
                //throw new InvalidOperationException($"La propiedad '{p.Name}' ya ha sido configurada.");
                return _properties.First(q => q.Property == p) is TDescriptor td ? td : default;

            var cpd = typeof(TDescriptor).New<TDescriptor>(p);
            _properties.Add(cpd);
            return cpd;
        }
    }
}