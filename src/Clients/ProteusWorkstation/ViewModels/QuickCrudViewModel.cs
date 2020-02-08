/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.ViewModels
{
    public class QuickCrudViewModel : CrudViewModelBasicBase, ICrudEditingViewModel, IReadEntityViewModel<ModelBase>, IPageViewModel, ICrudViewModel
    {
        /// <summary>
        /// Enumera los posibles modos de funcionamiento de este
        /// <see cref="QuickCrudViewModel"/>.
        /// </summary>
        public enum CrudMode : byte
        {
            /// <summary>
            /// Luego de guardar, se cerrará la página.
            /// </summary>
            Close,
            /// <summary>
            /// Luego de guardar, se creará una nueva entidad.
            /// </summary>
            New
        }

        private ModelBase? _parent;
        private CrudMode _mode;
        private CrudElement _element;
        private IEnumerable<CrudTool> _tools;

        public override CrudElement SelectedElement => _element;

        /// <summary>
        ///     Obtiene o establece el valor Entity.
        /// </summary>
        /// <value>El valor de Entity.</value>

        public ModelBase Entity
        {
            get => (ModelBase)SelectedElement.ViewModel.Entity!;
            private set
            {
                SelectedElement.ViewModel.Entity = value as ModelBase;
            }
        }

        public ICommand CancelCommand => CloseCommand;

        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Obtiene un valor que determina si este ViewModel puede ser
        /// cerrado.
        /// </summary>
        public bool Closeable => true;

        /// <summary>
        /// Obtiene un comando de cierre de este ViewModel.
        /// </summary>
        public SimpleCommand CloseCommand { get; private set; }

        /// <summary>
        /// Obtiene al contenedor visual cerrable de este ViewModel.
        /// </summary>
        public ICloseable Host { get; private set; }

        /// <summary>
        /// Obtiene el título de este ViewModel a mostrar en su
        /// correspondiente contenedor visual.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Enumera los <see cref="Launcher"/> a presentar para la vista de
        /// Crud cuando se está editando una entidad.
        /// </summary>
        public IEnumerable<Launcher> EditingLaunchers => GetLaunchers(CrudToolVisibility.Editing);

        public FrameworkElement? SelectedDetails => _element.Details;

        public FrameworkElement? SelectedEditor => _element.Editor;

        public ICommand CreateNew { get; private set; }

        public ICommand EditCurrent { get; private set; }

        public ICommand DeleteCurrent { get; private set; }

        public Visibility BusyV => Visibility.Collapsed;

        public Visibility NotBusyV => Visibility.Visible;

        public Visibility MultiModel => Visibility.Collapsed;

        public Visibility UniModel => Visibility.Visible;

        public bool EditMode => true;

        public bool NotEditMode => false;

        public bool NotBusy => !IsBusy;

        public Visibility EditVis => Visibility.Visible;

        public Visibility NotEditVis => Visibility.Collapsed;

        public IEnumerable<Launcher>? CreateCommands => Array.Empty<Launcher>();

        public ModelBase? Selection
        { 
            get => _element.ViewModel.Entity as ModelBase; 
            set => throw new InvalidOperationException();
        }

        public QuickCrudViewModel(ICloseable host, Type model, ModelBase? parent) : this(host, model, parent, CrudMode.Close)
        {
        }

        private void Init(ICloseable host, Type model, ModelBase? parent, CrudMode mode)
        {
            CreateNew = new ObservingCommand(this, OnCreate, CanCreate, nameof(SelectedElement));
            EditCurrent = new SimpleCommand(()=>throw new TamperException(), false);
            DeleteCurrent = new ObservingCommand(this, o => BusyOp(() => OnDelete(o)), CanDelete, nameof(SelectedElement));
            SaveCommand = new SimpleCommand(OnSave, true);
            CloseCommand = new SimpleCommand(Close);
            _parent = parent;
            _mode = mode;
            Host = host;
            _element = new CrudElement(model);
            _tools = CrudViewModelBase._allTools.Where(p => p.Available(model));
        }

        public QuickCrudViewModel(ICloseable host, Type model, ModelBase? parent, CrudMode mode)
        {
            Init(host, model, parent, mode);
            Entity = model.New<ModelBase>();
            ClearCtrls(Entity);
            Title = $"Nuevo {SelectedElement.Description.FriendlyName}";
        }

        public QuickCrudViewModel(ICloseable host, ModelBase entity, ModelBase? parent, CrudMode mode)
        {
            Init(host, entity.GetType().ResolveCollectionType().ResolveToDefinedType()!, parent, mode);
            Entity = entity;
            DisableIdCtrls();
            foreach (var k in _element.EditControls)
            {
                k.GetValue(k.Description.PropertySource == PropertyLocation.Model ? (object)entity! : SelectedElement!.ViewModel);
            }
            Title = $"Editar {Entity}";
        }

        private void OnSave() => BusyOp(OnSaveAsync());

        private async Task OnSaveAsync()
        {
            if (Precheck()) return;
            var t = await (Entity.IsNew ? Service!.AddAsync(Entity) : Service!.SaveAsync());
            if (t.Result == Result.Ok) 
            {
                await PostSave(Entity); 
                switch (_mode)
                {
                    case CrudMode.Close:Close(); break;
                    case CrudMode.New:
                        OnCreate(null);
                        break;
                }
            }
        }

        public void Close()
        {
            if (Entity?.IsNew ?? false) Service!.Rollback(Entity!);
            App.UiInvoke(Host.Close);
        }

        protected override ModelBase? GetParent() => _parent;

        private IEnumerable<Launcher> GetLaunchers(CrudToolVisibility flags)
        {
            return _tools.Where(p => p.Visibility.HasFlag(flags)).SelectMany(p => p.GetLaunchers(new[] { _element.Model }, this));
        }

        public bool CanCreate(Type t)
        {
            return _mode == CrudMode.New;
        }

        public bool CanEdit(ModelBase entity)
        {
            return false;
        }

        public bool CanDelete(ModelBase entity)
        {
            return SelectedElement?.Description?.CanDelete?.Invoke(entity) ?? true;
        }

        public void OnCreate(Type? t)
        {
            Entity = (t ?? SelectedElement.Model).New<ModelBase>();
            ClearCtrls(Entity);
        }

        /// <summary>
        /// Ejecuta una operación de eliminado de información de la
        /// colección activa.
        /// </summary>
        /// <param name="o">
        /// Elemento a eliminar.
        /// </param>
        public async void OnDelete(object? o)
        {
            if (!(o is ModelBase m)) return;
            var t = await PerformAsync(() => Service!.PurgeAsync(m));
            if (t.Result == Result.Ok) Close();
        }

        public void BusyDo(Task action)
        {
            BusyOp(action);
        }

        [Sugar] internal void OnCreate(object? o) => OnCreate(o as Type);
        [Sugar] internal bool CanCreate(object? o) => CanCreate(o as Type);
        [Sugar] internal bool CanEdit(object? o) => CanEdit(Selection as ModelBase);
        [Sugar] internal bool CanDelete(object? o) => CanDelete(Selection as ModelBase);
    }
}