/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    /// ViewModel que controla la gestión de la ventana de Crud.
    /// </summary>
    /// <typeparam name="TService">
    /// Tipo de servicio enlazado.
    /// </typeparam>
    public class CrudViewModel<TService> : PageViewModel, ICrudCollectionViewModel, ISearchViewModel, IEditingCrudViewModel where TService : Service, new()
    {
        private readonly IEnumerable<CrudTool> _tools;
        private readonly Type _model;
        private bool _willSearch = true;
        private string? _searchQuery;
        private bool _isSearching;
        private ICollectionView? _results;
        private IEnumerable<ModelBase>? _enumerableResults;
        private GridLength _smallPanelSize = new GridLength(2.5, GridUnitType.Star);
        private GridLength _largePanelSize = new GridLength(1, GridUnitType.Star);
        private readonly CrudCollectionViewModelBase _implementation;

        public double EditorWidth => Settings.Default.EditorWidth;

        /// <summary>
        /// Obtiene o establece el valor EnumerableResults.
        /// </summary>
        /// <value>El valor de EnumerableResults.</value>
        public IEnumerable<ModelBase>? EnumerableResults
        {
            get => _enumerableResults ?? Source;
            private set => Change(ref _enumerableResults, value);
        }

        /// <summary>
        /// Enumera los <see cref="Launcher"/> a presentar para la vista de
        /// Crud cuando no hay una entidad seleccionada.
        /// </summary>
        public IEnumerable<Launcher> UnselectedLaunchers => GetLaunchers(CrudToolVisibility.Unselected);

        /// <summary>
        /// Enumera los <see cref="Launcher"/> a presentar para la vista de
        /// Crud cuando hay una entidad seleccionada.
        /// </summary>
        public IEnumerable<Launcher> SelectedLaunchers => GetLaunchers(CrudToolVisibility.Selected);

        public Visibility SelectedLaunchersVisibility => SelectedLaunchers.Any() ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Enumera los <see cref="Launcher"/> a presentar para la vista de
        /// Crud cuando se está editando una entidad.
        /// </summary>
        public IEnumerable<Launcher> EditingLaunchers => GetLaunchers(CrudToolVisibility.Editing);

        /// <summary>
        /// Obtiene un valor que indica si este ViewModel se encuentra 
        /// ocupado.
        /// </summary>
        public new bool IsBusy => _implementation.IsBusy;

        /// <summary>
        /// Obtiene al elemento selector de la ventana.
        /// </summary>
        public ItemsControl Selector => ((ICrudCollectionViewModel)_implementation).Selector;

        /// <summary>
        /// Obtiene o establece al elemento seleccionado.
        /// </summary>
        public ModelBase? Selection { get => ((ICrudCollectionViewModel)_implementation).Selection; set => ((ICrudCollectionViewModel)_implementation).Selection = value; }

        /// <summary>
        /// Obtiene un <see cref="ViewBase"/> que define la apariencia de
        /// un selector <see cref="ListView"/> cuando esta ventana de CRUD
        /// controla únicamente un modelo de datos.
        /// </summary>
        public ViewBase? ColumnsView => ((ICrudCollectionViewModel)_implementation).ColumnsView;

        /// <summary>
        /// Enumera el orígen de datos establecido para este Crud.
        /// </summary>
        public ICollection<ModelBase> Source => ((ICrudCollectionViewModel)_implementation).Source;

        /// <summary>
        /// Obtiene la ventana de detalles de la entidad seleccionada.
        /// </summary>
        public FrameworkElement? SelectedDetails => ((ICrudCollectionViewModel)_implementation).SelectedDetails;

        /// <summary>
        /// Obtiene el editor a utlizar para editar a la entidad seleccionada.
        /// </summary>
        public FrameworkElement? SelectedEditor => ((ICrudCollectionViewModel)_implementation).SelectedEditor;

        /// <summary>
        /// Obtiene un <see cref="CrudElement"/> con información sobre los
        /// componentes relacionados al modelo de datos de la entidad
        /// seleccionada.
        /// </summary>
        public CrudElement? SelectedElement => ((ICrudCollectionViewModel)_implementation).SelectedElement;

        /// <summary>
        ///     Obtiene o establece el valor LeftPanelSize.
        /// </summary>
        /// <value>El valor de LeftPanelSize.</value>
        public GridLength LeftPanelSize
        {
            get => EditMode ? _largePanelSize: _smallPanelSize;
            set
            {
                if (EditMode)
                {
                    Change(ref _largePanelSize, value);
                }
                else
                {
                    Change(ref _smallPanelSize, value);
                }
            }
        }

        /// <summary>
        ///     Obtiene o establece el valor RIghtPanelSize.
        /// </summary>
        /// <value>El valor de RIghtPanelSize.</value>
        public GridLength RightPanelSize
        {
            get => EditMode ? _smallPanelSize : _largePanelSize;
            set
            {
                if (EditMode)
                {
                    Change(ref _smallPanelSize, value);
                }
                else
                {
                    Change(ref _largePanelSize, value);
                }
            }
        }

        /// <summary>
        /// Comando para la creación de nuevas entidades.
        /// </summary>
        public ICommand CreateNew => ((ICrudCollectionViewModel)_implementation).CreateNew;

        /// <summary>
        /// Comando para la edición de la entidad actualmente seleccionada.
        /// </summary>
        public ICommand EditCurrent => ((ICrudCollectionViewModel)_implementation).EditCurrent;

        /// <summary>
        /// Comando para la eliminación de la entidad actualmente
        /// seleccionada.
        /// </summary>
        public ICommand DeleteCurrent => ((ICrudCollectionViewModel)_implementation).DeleteCurrent;

        /// <summary>
        /// Comando de guardado de entidades, tanto nuevas como editadas.
        /// </summary>
        public ICommand SaveCommand => ((ICrudCollectionViewModel)_implementation).SaveCommand;

        /// <summary>
        /// Comando que cancela la creación o edición de una entidad.
        /// </summary>
        public ICommand CancelCommand => ((ICrudCollectionViewModel)_implementation).CancelCommand;

        /// <summary>
        /// Obtiene un valor de visibilidad aplicable mientras el ViewModel se encuentre ocupado.
        /// </summary>
        public Visibility BusyV => ((ICrudCollectionViewModel)_implementation).BusyV;

        /// <summary>
        /// Obtiene un valor de visibilidad aplicable mientras el ViewModel no se encuentre ovupado.
        /// </summary>
        public Visibility NotBusyV => ((ICrudCollectionViewModel)_implementation).NotBusyV;

        /// <summary>
        /// Obtiene un valor de visibilidad aplicable cuando el ViewModel administre la creación de múltiples modelos.
        /// </summary>
        public Visibility MultiModel => ((ICrudCollectionViewModel)_implementation).MultiModel;

        /// <summary>
        /// Obtiene un valor de visibilidad aplicacble cuando el ViewModel administre la creación de un único modelo.
        /// </summary>
        public Visibility UniModel => ((ICrudCollectionViewModel)_implementation).UniModel;

        /// <summary>
        /// Obtiene un valor que indica si el ViewModel se encuentra actualmente en modo de edición.
        /// </summary>
        public bool EditMode => ((ICrudCollectionViewModel)_implementation)?.EditMode ?? false;

        /// <summary>
        /// Obtiene un valor que indica si el ViewModel no se encuentra actualmente en modo de edición.
        /// </summary>
        public bool NotEditMode => ((ICrudCollectionViewModel)_implementation).NotEditMode;

        /// <summary>
        /// Obtiene un valor que indica si el ViewModel no está ocupado.
        /// </summary>
        public bool NotBusy => ((ICrudCollectionViewModel)_implementation).NotBusy;

        /// <summary>
        /// Obtiene un valor de visibilidad aplicable cuando el ViewModel se encuentre en modo de edición.
        /// </summary>
        public Visibility EditVis => ((ICrudCollectionViewModel)_implementation).EditVis;

        /// <summary>
        /// Obtiene un valor de visibilidad aplicable cuando el ViewModel no se encuentre en modo de edición.
        /// </summary>
        public Visibility NotEditVis => ((ICrudCollectionViewModel)_implementation).NotEditVis;

        /// <summary>
        /// Enumeración de comandos para la creación de entidades cuando
        /// este ViewModel administra dos o más modelos de datos.
        /// </summary>
        public IEnumerable<Launcher>? CreateCommands => ((ICrudCollectionViewModel)_implementation).CreateCommands;

        /// <summary>
        /// Obtiene el título de esta ventana.
        /// </summary>
        public override string Title
        {
            get
            {
                if (EditMode)
                {
                    return (Selection?.IsNew ?? true)
                        ? $"Nuevo {SelectedElement?.Description.FriendlyName ?? "elemento"}"
                        : $"Editar {SelectedElement?.Description.FriendlyName ?? "elemento"} {Selection?.StringId}";
                }
                return base.Title;
            }
            set => base.Title = value;
        }

        /// <summary>
        /// Obtiene o establece un valor que indica si esta ventana puede
        /// cerrarse.
        /// </summary>
        public override bool Closeable
        {
            get => base.Closeable && !EditMode;
            protected set => base.Closeable = value;
        }

        /// <summary>
        /// Obtiene o establece el valor SearchQuery.
        /// </summary>
        /// <value>El valor de SearchQuery.</value>
        public string? SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (!Change(ref _searchQuery, value)) return;
                WillSearch = true;
            }
        }

        /// <summary>
        /// Obtiene una colección con los resultados de la búsqueda.
        /// </summary>
        public ICollectionView? Results
        {
            get => _results;
            private set
            {
                if (Change(ref _results, value)) _results?.Refresh();
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si al ejecutar
        /// <see cref="SearchCommand"/> se hará una búsqueda o se limpiará
        /// la búsqueda actual.
        /// </summary>
        public bool WillSearch
        {
            get => _willSearch;
            private set => Change(ref _willSearch, value);
        }

        /// <summary>
        /// Obtiene el comando relacionado a la acción Search.
        /// </summary>
        /// <returns>El comando Search.</returns>
        public ObservingCommand SearchCommand { get; private set; } = null!;

        /// <summary>
        /// Obtiene la etiqueta a utilizar para mostrar sobre el botón de
        /// búsqueda.
        /// </summary>
        public string SearchLabel => _willSearch ? "🔍" : "❌";

        /// <summary>
        /// Obtiene una cadena que describe la cantidad de resultados encontrados.
        /// </summary>
        public string ResultsDetails => Results is null ? $"Hay más de {Settings.Default.RowLimit} elementos. Inicie una búsqueda para continuar." : WillSearch ? $"{Source.Count()} elementos{(Source.Count() > Settings.Default.RowLimit ? $" (limitado a los últimos {Settings.Default.RowLimit})" : null)}" : $"{Results!.Count()} elementos, {Source.Count()} en total";

        /// <summary>
        /// Obtiene o establece el valor IsSearching.
        /// </summary>
        /// <value>El valor de IsSearching.</value>
        public bool IsSearching
        {
            get => _isSearching;
            set => Change(ref _isSearching, value);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CrudViewModel{TService}"/>.
        /// </summary>
        /// <param name="host">
        /// Host visual de la ventana asociada a este ViewModel.
        /// </param>
        /// <param name="model">
        /// Modelo único de datos.
        /// </param>
        public CrudViewModel(ICloseable host, Type model) : base(host)
        {
            _model = model;
            _implementation = new DbBoundCrudViewModel(model);
            _tools = CrudViewModelBase._allTools.Where(p => p.Available(model));
            Init();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CrudViewModel{TService}"/>.
        /// </summary>
        /// <param name="host">Host visual de la ventana asociada a este ViewModel.</param>
        /// <param name="source">Origen de datos a utilizar.</param>
        /// <param name="models">Modelos asociados de datos.</param>
        public CrudViewModel(ICloseable host, IQueryable<ModelBase> source, params Type[] models) : base(host, true)
        {
            _model = models.First();
            _implementation = new DbBoundCrudViewModel(source, models);
            _tools = CrudViewModelBase._allTools.Where(p => p.Available(models));
            Init();
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
        public bool CanCreate(Type t) => ((ICrudCollectionViewModel)_implementation).CanCreate(t);

        /// <summary>
        /// Determina si es posible editar a la entidad seleccionada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// <see langword="true"/> si es posible editar la entidad
        /// seleccionada, <see langword="false"/> en caso contrario.
        /// </returns>
        public bool CanEdit(ModelBase entity) => ((ICrudCollectionViewModel)_implementation).CanEdit(entity);

        /// <summary>
        /// Determina si es posible eliminar a la entidad seleccionada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// <see langword="true"/> si es posible eliminar la entidad
        /// seleccionada, <see langword="false"/> en caso contrario.
        /// </returns>
        public bool CanDelete(ModelBase entity) => ((ICrudCollectionViewModel)_implementation).CanDelete(entity);

        /// <summary>
        /// Ejecuta una acción que Crea una nueva entidad.
        /// </summary>
        /// <param name="t">Modelo de datos a crear.</param>
        public void OnCreate(Type? t)
        {
            ((ICrudCollectionViewModel)_implementation).OnCreate(t);
        }

        /// <summary>
        /// Ejecuta una operación colocando a este 
        /// <see cref="ICrudEditingViewModel"/> en estado de ocupado.
        /// </summary>
        /// <param name="action">Tarea a ejecutar.</param>
        public void BusyDo(Task action) => _implementation.BusyDo(action);

        /// <summary>
        /// Limpia los resultados de la búsqueda.
        /// </summary>
        public void ClearSearch()
        {
            EnumerableResults = null;
            Results = Source.Count() <= Settings.Default.RowLimit ? CollectionViewSource.GetDefaultView(Source) : null;
            SearchQuery = null;
        }

        private async void OnSearch()
        {
            if (WillSearch && !SearchQuery.IsEmpty()) await PerformSearch();
            else ClearSearch();
            Notify(nameof(ResultsDetails));
        }
        private async Task PerformSearch()
        {
            IsSearching = true;
            var l = (await Internal.Query(SearchQuery!, _model).ToListAsync()).Cast<ModelBase>().ToList();
            var ll = new List<ModelBase>();
            foreach (var j in Objects.FindAllObjects<IModelLocalSearchFilter>().Where(p => p.UsableFor(_model)))
            {
                ll = ll.Concat(j.Filter(l, SearchQuery!)).ToList();
            }
            l = ll.Distinct().ToList();
            EnumerableResults = l;
            Results = CollectionViewSource.GetDefaultView(l);
            IsSearching = false;
            WillSearch = false;
        }
        private IEnumerable<Launcher> GetLaunchers(CrudToolVisibility flags)
        {
            return _tools.Where(p => p.Visibility.HasFlag(flags)).SelectMany(p => p.GetLaunchers(_model, this));
        }
        private void Init()
        {
            Settings.Default.PropertyChanged += Default_PropertyChanged;
            _implementation.ForwardChange(this);
            RegisterPropertyChangeTrigger(nameof(LeftPanelSize), nameof(EditMode));
            RegisterPropertyChangeTrigger(nameof(RightPanelSize), nameof(EditMode));
            RegisterPropertyChangeTrigger(nameof(Title), nameof(EditMode));
            RegisterPropertyChangeBroadcast(nameof(EditMode), nameof(Closeable));
            RegisterPropertyChangeTrigger(nameof(ResultsDetails), nameof(Results), nameof(Source));
            SearchCommand = CreateSearchCommand();
            ClearSearch();
        }
        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Notify(e.PropertyName);
        }
        private ObservingCommand CreateSearchCommand()
        {
            RegisterPropertyChangeBroadcast(nameof(WillSearch), nameof(SearchLabel));
            return new ObservingCommand(this, OnSearch)
                .ListensToProperty(() => SearchQuery!)
                .SetCanExecute(() => !SearchQuery.IsEmpty());
        }

        ~CrudViewModel()
        {
            Settings.Default.PropertyChanged -= Default_PropertyChanged;
        }
    }
}