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
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using static TheXDS.MCART.Types.Extensions.ObservingCommandExtensions;
using static TheXDS.MCART.Types.Extensions.StringExtensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.ViewModels
{
    /// <summary>
    /// ViewModel que controla el Widget de edición y selección de 
    /// entidades a partir de una lista.
    /// </summary>
    public class ObjectEditorViewModel : CrudViewModelBase, ISearchViewModel
    {
        private string _fieldIcon = null!;
        private string _fieldName = null!;
        private bool _canSelect;
        private bool _selectMode;
        private ModelBase? _tempSelection;
        private Type? _activeModel;
        private string? _searchQuery;
        private bool _canSearch = true;
        private bool _isSearching;
        private ICollectionView? _results;
        private IEnumerable<ModelBase>? _enumerableResults;

        /// <summary>
        /// Obtiene o establece el valor EnumerableResults.
        /// </summary>
        /// <value>El valor de EnumerableResults.</value>
        public IEnumerable<ModelBase>? EnumerableResults
        {
            get => _enumerableResults ?? Proteus.Infer(ActiveModel)?.All(ActiveModel!);
            private set => Change(ref _enumerableResults, value);
        }

        /// <summary>
        /// Obtiene un valor que determina si se habilita los controles de
        /// edición del widget.
        /// </summary>
        public bool ShowEditControls { get; }

        /// <summary>
        /// Enumera los modelos disponibles para seleccionar en la búsqueda.
        /// </summary>
        public IList<Type> SelectableModels { get; }

        /// <summary>
        /// Obtiene o establece el valor ActiveModel.
        /// </summary>
        /// <value>El valor de ActiveModel.</value>
        public Type? ActiveModel
        {
            get => _activeModel;
            set
            {
                if (!Change(ref _activeModel, value)) return;
                ClearSearch();
            }
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
        /// Obtiene un valor que indica si es posible cambiar el modelo 
        /// seleccionado.
        /// </summary>
        public bool CanChangeModel => SelectableModels.Count > 1;

        /// <summary>
        /// Obtiene la vista columnar a utilizar para mostrar objetos en la lista de resultados.
        /// </summary>
        public ViewBase? ColumnsView
        {
            get
            {
                if (ActiveModel is null) return null;
                if (!(CrudElement.GetDescription(ActiveModel)?.ListColumns is { } c)) return null;
                var v = new GridView();
                foreach (var j in c)
                {
                    v.Columns.Add(j);
                }
                return v;
            }
        }

        /// <summary>
        /// Obtiene una colección con los resultados de la búsqueda.
        /// </summary>
        public ICollectionView? Results
        {
            get => _results;
            private set => Change(ref _results, value);
        }

        /// <summary>
        /// Obtiene un valor que indica si al ejecutar
        /// <see cref="SearchCommand"/> se hará una búsqueda o se limpiará
        /// la búsqueda actual.
        /// </summary>
        public bool WillSearch
        {
            get => _canSearch;
            private set => Change(ref _canSearch, value);
        }

        /// <summary>
        /// Obtiene la etiqueta a utilizar para mostrar sobre el botón de
        /// búsqueda.
        /// </summary>
        public string SearchLabel => WillSearch ? "🔍" : "❌";

        /// <summary>
        /// Obtiene el comando relacionado a la acción Search.
        /// </summary>
        /// <returns>El comando Search.</returns>
        public ObservingCommand SearchCommand { get; }

        /// <summary>
        /// Obtiene la etiqueta del modelo para el cual se ha construido 
        /// este control.
        /// </summary>
        public string ModelLabel { get; }

        /// <summary>
        /// Obtiene el origen de selección de este <see cref="ListEditorViewModel"/>.
        /// </summary>
        public ICollection<ModelBase>? SelectionSource { get; }

        /// <summary>
        /// Obtiene el comando que agrega elementos desde la lista de
        /// origen de selección a la colección del modelo de datos.
        /// </summary>
        public SimpleCommand SelectCommand { get; }

        /// <summary>
        /// Obtiene el comando que acepta la selección.
        /// </summary>
        public ICommand OkSelectCommand { get; }

        /// <summary>
        /// Obtiene el comando que cancela la adición de elementos
        /// seleccionados desde una lista.
        /// </summary>
        public ICommand CancelSelectCommand { get; }

        /// <summary>
        /// Obtiene o establece un valor que configura este
        /// <see cref="ListEditorViewModel"/> para agregar elementos desde
        /// una lista.
        /// </summary>
        public bool SelectMode
        {
            get => _selectMode;
            set => Change(ref _selectMode, value);
        }

        /// <summary>
        /// Obtiene o establece el valor de selección temporal de la lista
        /// de búsqueda.
        /// </summary>
        public ModelBase? TempSelection
        { 
            get=> _tempSelection;
            set => Change(ref _tempSelection, value);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ObjectEditorViewModel"/>.
        /// </summary>
        /// <param name="description">
        /// Descripción de propiedad con la cual generar el control.
        /// </param>
        /// <param name="models">
        /// Modelos aceptados por el valor de la propiedad.
        /// </param>
        public ObjectEditorViewModel(IEntityViewModel parentVm, IObjectPropertyDescription description, params Type[] models) : this(parentVm, AppInternal.GetSource(description.Source), description, models) { }

        private void OnCancelSelect()
        {
            SelectMode = false;
            IsBusy = false;
            TempSelection = null;
            ClearSearch();
        }

        private void OnOkSelect()
        {
            Selection = TempSelection;
            _description.Property.SetValue(_description.PropertySource == PropertyLocation.ViewModel ? _parentVm : _parentVm.Entity, TempSelection);
            OnCancelSelect();
        }

        private readonly IObjectPropertyDescription _description;
        private readonly IEntityViewModel _parentVm;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ObjectEditorViewModel"/>.
        /// </summary>
        /// <param name="selectionSource">
        /// Origen de datos para la lista de selección.
        /// </param>
        /// <param name="description">
        /// Descripción de propiedad con la cual generar el control.
        /// </param>
        /// <param name="models">
        /// Modelos aceptados por el valor de la propiedad.
        /// </param>
        public ObjectEditorViewModel(IEntityViewModel parentVm, ICollection<ModelBase>? selectionSource, IObjectPropertyDescription description, params Type[] models) : base(models)
        {
            _description = description;
            _parentVm = parentVm;
            FieldName = description.Label;
            FieldIcon = description.Icon;
            CanSelect = description.Selectable;
            ShowEditControls = description.Creatable;
            SelectCommand = new SimpleCommand(OnSelect);
            OkSelectCommand = new SimpleCommand(OnOkSelect);
            CancelSelectCommand = new SimpleCommand(OnCancelSelect);

            SelectableModels = description.ChildModels?.ToList()
                ?? description.PropertyType.Derivates().Select(p => p.ResolveToDefinedType()!).Distinct().Where(TypeExtensions.IsInstantiable).OrNull()?.ToList()
                ?? new[] { description.PropertyType }.ToList();

            ModelLabel = description.Label;
            ActiveModel = SelectableModels.FirstOrDefault();
            SelectionSource = description.UseVmSource ? description.VmSource(parentVm, this) : selectionSource;

            RegisterPropertyChangeBroadcast(nameof(Selection), nameof(DisplayValue));
            RegisterPropertyChangeBroadcast(nameof(ActiveModel), nameof(ColumnsView));
            RegisterPropertyChangeBroadcast(nameof(WillSearch), nameof(SearchLabel));

            SearchCommand = new ObservingCommand(this, OnSearch);
            SearchCommand.ListensToProperty(() => SearchQuery!);
            SearchCommand.ListensToProperty(() => ActiveModel);
            SearchCommand.SetCanExecute(() => !SearchQuery.IsEmpty() && ActiveModel != null);
        }

        private void OnSelect()
        {
            OnCancel();
            IsBusy = true;
            SelectMode = true;
        }

        /// <summary>
        /// Obtiene un valor que indica si este
        /// <see cref="ListEditorViewModel"/> permite adicionar elementos
        /// existentes a la colección del modelo de datos.
        /// </summary>
        public bool CanSelect
        {
            get => _canSelect;
            private set
            {
                _canSelect = value;
                SelectCommand?.SetCanExecute(value);
            }
        }

        /// <summary>
        /// Elimina el objeto de la selección.
        /// </summary>
        /// <param name="o">
        /// Valor a eliminar.
        /// </param>
        public override void OnDelete(object? o)
        {
            Selection = null;
        }

        /// <summary>
        /// Ejecuta una operación de guardado de la entidad actualmente en 
        /// edición.
        /// </summary>
        /// <param name="entity">
        /// Entidad en edición.
        /// </param>
        /// <returns>
        /// Este método siempre devuelve <see cref="DetailedResult.Ok"/>.
        /// </returns>
        protected override Task<DetailedResult> PerformSave(ModelBase entity)
        {
            return Task.FromResult(DetailedResult.Ok);
        }

        /// <summary>
        /// Obtiene al pariente de la entidad actualmente seleccionada.
        /// </summary>
        /// <returns>
        /// Este método siempre devuelve <see langword="null"/>.
        /// </returns>
        protected override ModelBase? GetParent()
        {
            return null;
        }

        /// <summary>
        /// Ejecuta operaciones adicionales posteriores al guardado de una
        /// entidad.
        /// </summary>
        protected override async Task PostSave(ModelBase e)
        {
            await base.PostSave(e);
            Notify(nameof(DisplayValue));
        }

        /// <summary>
        /// Obtiene el ícono configurado para mostrar del campo
        /// correspondiente a la colección subyacente del modelo de datos.
        /// </summary>
        public string FieldIcon
        {
            get => _fieldIcon;
            internal set => Change(ref _fieldIcon, value);
        }

        /// <summary>
        /// Obtiene un valor amigable para mostrar en la UI de la aplicación.
        /// </summary>
        public string DisplayValue => Selection?.ToString() ?? "-";

        /// <summary>
        /// Obtiene el nombre configurado para mostrar del campo
        /// correspondiente a la colección subyacente del modelo de datos.
        /// </summary>
        public string FieldName
        {
            get => _fieldName;
            internal set => Change(ref _fieldName, value);
        }

        /// <summary>
        /// Limpia los resultados de la búsqueda.
        /// </summary>
        public async void ClearSearch()
        {
            if (SelectionSource?.Any() ?? false)
            {
                Results = CollectionViewSource.GetDefaultView(SelectionSource);
            }
            else if (Proteus.Infer(ActiveModel!) is { } svc)
            {          
                var q = svc.All(ActiveModel!);
                if (q.Count() <= Settings.Default.RowLimit)
                {
                    try
                    {
                        var re = await q.ToListAsync();
                        Results = CollectionViewSource.GetDefaultView(re);
                    }
                    catch 
                    {
                        Results = null;
                    }
                }
                else
                {
                    Results = null;
                }
            }
            EnumerableResults = null;
            SearchQuery = null;
        }

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
        /// Obtiene una cadena que describe los detalles de la búsqueda.
        /// </summary>
        public string ResultsDetails => string.Empty;

        private async void OnSearch()
        {
            Selection = null;
            if (WillSearch && !SearchQuery.IsEmpty() && ActiveModel != null) await PerformSearch();
            else ClearSearch();
        }

        private async Task PerformSearch()
        {
            IsSearching = true;
            List<ModelBase> l;
            if (SelectionSource.Any())
            {
                l = SelectionSource.ToList();
            }
            else if (Proteus.Infer(ActiveModel!) is { })
            {
                l = (await Internal.Query(SearchQuery!, ActiveModel!).ToListAsync()).Cast<ModelBase>().ToList();
            }
            else { return; }
            var ll = new List<ModelBase>();
            foreach (var j in Objects.FindAllObjects<IModelLocalSearchFilter>().Where(p=>p.UsableFor(ActiveModel!)))
            {
                ll = ll.Concat(j.Filter(l, SearchQuery!)).ToList();
            }
            l = ll.Distinct().ToList();
            EnumerableResults = l;
            Results = CollectionViewSource.GetDefaultView(l);
            Results.Refresh();
            IsSearching = false;
            WillSearch = false;
        }
    }
}