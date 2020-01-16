/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models.Base;
using System.Windows.Controls;
using System.Collections;
using TheXDS.Proteus.Crud.Base;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using System.ComponentModel;
using System.Windows.Data;
using TheXDS.Proteus.Misc;
using System.Data.Entity;

namespace TheXDS.Proteus.ViewModels
{

    /// <summary>
    /// ViewModel que controla la adición de elementos a una colección de
    /// un modelo de datos.
    /// </summary>
    public class ListEditorViewModel : CrudCollectionViewModelBase
    {
        private readonly HashSet<ModelBase> _addedFromSelection = new HashSet<ModelBase>();
        private SelectionMode _selectMode;
        private bool _addMode;
        private IList _addSelections;
        private bool _canSelect;
        private string _fieldName;
        private bool _canAdd;
        private string _fieldIcon;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ListEditorViewModel"/>.
        /// </summary>
        /// <param name="selectionSource">
        /// Origen de selección.
        /// </param>
        /// <param name="collection">
        /// Colección subyacente a administrar.
        /// </param>
        /// <param name="models">
        /// Modelos creables desde este <see cref="ListEditorViewModel"/>.
        /// </param>
        public ListEditorViewModel(ICollection<ModelBase> selectionSource, ICollection<ModelBase> collection, params Type[] models) : base(collection, models, nameof(Source))
        {
            SelectionSource = selectionSource;
            AddCommand = new SimpleCommand(OnSelect);
            RemoveCommand = new SimpleCommand(OnRemove);//new ObservingCommand(this, OnRemove, CanRemove, nameof(Selection));
            OkAddCommand = new SimpleCommand(OnOkAdd);
            CancelAddCommand = new SimpleCommand(OnCancelAdd);
            RegisterPropertyChangeBroadcast(nameof(CanAdd), nameof(CanAddAndSelect));
            RegisterPropertyChangeBroadcast(nameof(CanSelect), nameof(CanAddAndSelect));            
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ListEditorViewModel"/>.
        /// </summary>
        /// <param name="description">
        /// Descripción bajo la cual se debe instanciar este objeto.
        /// </param>
        public ListEditorViewModel(IListPropertyDescription description) : this(description, new[] { description.Property.PropertyType })
        {
        }

        public ListEditorViewModel(IListPropertyDescription description, params Type[] models) : this(description.Source?.ToList(), new List<ModelBase>(), models)
        {
            CanAdd = description.Creatable;
            CanSelect = description.Selectable;
            FieldName = description.Label;
            FieldIcon = description.Icon;
            CustomColumns.AddRange(description.Columns);
            if (Selector is ListView lv)
            {
                lv.SelectionChanged += ListViewSelector_SelectionChanged;
            }
        }
        private void ListViewSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(Selector is ListView lv)) return;
            Selection = lv.SelectedItems.ToGeneric().FirstOrDefault();
        }

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
        /// Obtiene el ícono configurado para mostrar del campo
        /// correspondiente a la colección subyacente del modelo de datos.
        /// </summary>
        public string FieldIcon
        {
            get => _fieldIcon;
            internal set => Change(ref _fieldIcon, value);
        }

        /// <summary>
        /// Obtiene un valor que indica si este
        /// <see cref="ListEditorViewModel"/> permite adicionar elementos
        /// existentes a la colección del modelo de datos.
        /// </summary>
        public bool CanSelect
        {
            get => _canSelect;
            internal set
            {
                _canSelect = value;
                AddCommand.SetCanExecute(value);
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si es posible crear una nueva
        /// entidad del tipo especificado.
        /// </summary>
        /// <param name="t">Modelo de datos a instanciar.</param>
        /// <returns>
        /// <see langword="true"/> si es posible crear una nueva entidad de
        /// tipo <paramref name="t"/>, <see langword="false"/> en caso
        /// contrario.
        /// </returns>
        public override bool CanCreate(Type t) => CanAdd && base.CanCreate(t);

        /// <summary>
        /// Obtiene un valor que indica si este
        /// <see cref="ListEditorViewModel"/> permite adicionar nuevas
        /// entidades.
        /// </summary>
        public bool CanAdd
        {
            get => _canAdd;
            internal set
            {
                Change(ref _canAdd, value);                
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si es posible agregar y seleccionar
        /// elementos existentes para agregar a la colección del modelo de 
        /// datos.
        /// </summary>
        public bool CanAddAndSelect => CanAdd && CanSelect;

        private void OnCancelAdd()
        {
            AddSelections?.Clear();
            Unselected?.Invoke(this, EventArgs.Empty);
            AddMode = false;
            IsBusy = false;
        }

        private void OnOkAdd()
        {
            foreach (var j in AddSelections?.OfType<ModelBase>() ?? Array.Empty<ModelBase>())
            {
                Source.Add(j);
                _addedFromSelection.Add(j);
            }
            OnCancelAdd();
            Notify(nameof(Source));
        }

        private bool CanRemove()
        {
            return Selection is ModelBase s && (_addedFromSelection.Contains(s) || !s.IsNew);
        }

        private void OnRemove()
        {
            if (!(Selection is ModelBase s)) return;

            Source.Remove(s);
            _addedFromSelection.Remove(s);
            Notify(nameof(Source));
        }

        private void OnSelect()
        {
            OnCancel();
            IsBusy = true;
            AddMode = true;
        }

        /// <summary>
        /// Agrega un elemento a la colección del modelo de datos.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override Task<DetailedResult> PerformSave(ModelBase entity)
        {
            if (entity.IsNew && NewMode)
            {
                Source.Add(entity);
            }
            else
            {
                Source.RefreshItem(entity);
            }
            return Task.FromResult(DetailedResult.Ok);
        }

        /// <summary>
        /// Ejecuta una operación de eliminado de información de la
        /// colección activa.
        /// </summary>
        /// <param name="o">
        /// Elemento a eliminar.
        /// </param>
        protected override void OnDelete(object o)
        {
            //if (!(o is ModelBase s)) return;
            //Source.Remove(s);
            //_addedFromSelection.Remove(s);
            //Notify(nameof(Source));

            OnRemove();
        }

        /// <summary>
        /// Obtiene o establece el modo de selección a utilizar para la
        /// lista de origen de selección.
        /// </summary>
        public SelectionMode SelectMode
        {
            get => _selectMode;
            set => Change(ref _selectMode, value);
        }

        /// <summary>
        /// Obtiene el comando que agrega elementos desde la lista de
        /// origen de selección a la colección del modelo de datos.
        /// </summary>
        public SimpleCommand AddCommand { get; }

        /// <summary>
        /// Obtiene el comando que quita un elemento de la colección del
        /// modleo de datos.
        /// </summary>
        public ICommand RemoveCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand OkAddCommand { get; }

        /// <summary>
        /// Obtiene el comando que cancela la adición de elementos
        /// seleccionados desde una lista.
        /// </summary>
        public ICommand CancelAddCommand { get; }

        /// <summary>
        /// Obtiene o establece un valor que configura este
        /// <see cref="ListEditorViewModel"/> para agregar elementos desde
        /// una lista.
        /// </summary>
        public bool AddMode
        {
            get => _addMode;
            set => Change(ref _addMode, value);
        }

        /// <summary>
        /// Obtiene o establece la lista de elementos seleccionados para
        /// agregar a la colección del modelo de datos.
        /// </summary>
        public IList AddSelections
        {
            get => _addSelections;
            set => Change(ref _addSelections, value);
        }

        /// <summary>
        /// Obtiene el origen de selección de este <see cref="ListEditorViewModel"/>.
        /// </summary>
        public ICollection<ModelBase> SelectionSource { get; }

        /// <summary>
        /// Ocurre cuando no hay nada seleccionado.
        /// </summary>
        public event EventHandler Unselected;







        private bool _willSearch = true;
        private string? _searchQuery;
        private bool _isSearching;
        private ICollectionView? _results;
        private Type? _activeModel;

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
        /// Limpia los resultados de la búsqueda.
        /// </summary>
        public void ClearSearch()
        {
            Results = Source.Count() <= Proteus.Settings?.RowLimit ? CollectionViewSource.GetDefaultView(Source) : null;
            SearchQuery = null;
        }

        /// <summary>
        /// Obtiene una cadena que describe la cantidad de resultados encontrados.
        /// </summary>
        public string ResultsDetails => Results is null ? $"Hay más de {Proteus.Settings?.RowLimit} elementos. Inicie una búsqueda para continuar." : WillSearch ? $"{Source.Count()} elementos{(Source.Count() > Proteus.Settings?.RowLimit ? $" (limitado a los últimos {Proteus.Settings?.RowLimit})" : null)}" : $"{Results!.Count()} elementos, {Source.Count()} en total";

        /// <summary>
        /// Obtiene o establece el valor IsSearching.
        /// </summary>
        /// <value>El valor de IsSearching.</value>
        public bool IsSearching
        {
            get => _isSearching;
            set => Change(ref _isSearching, value);
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
            Results = CollectionViewSource.GetDefaultView(await Internal.Query(SearchQuery!, ActiveModel!).ToListAsync());
            IsSearching = false;
            WillSearch = false;
        }
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

    }
}