/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.ViewModels.Base;
using System;
using System.Threading.Tasks;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Crud.Base;
using System.Collections.Generic;
using TheXDS.MCART.ViewModel;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;
using System.Data.Entity;
using System.Windows.Controls;
using System.Windows.Data;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Reporting;
using static TheXDS.MCART.Types.Extensions.ObservingCommandExtensions;
using static TheXDS.MCART.Types.Extensions.StringExtensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.ViewModels
{
    public class ObjectEditorViewModel : CrudViewModelBase
    {
        private string _fieldIcon;
        private string _fieldName;
        private bool _canSelect;
        private bool _selectMode;
        private object? _tempSelection;
        private Type? _activeModel;
        private string? _searchQuery;
        private bool _canSearch;
        private bool _isSearching;
        private ICollectionView? _results;

        /// <summary>
        ///     Enumera los modelos disponibles para seleccionar en la búsqueda.
        /// </summary>
        public IList<Type> Models { get; }

        /// <summary>
        ///     Obtiene o establece el valor ActiveModel.
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
        ///     Obtiene o establece el valor SearchQuery.
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
        ///     Obtiene un valor que indica si es posible cambiar el modelo 
        ///     seleccionado.
        /// </summary>
        public bool CanChangeModel => Models.Count > 1;

        /// <summary>
        ///     Obtiene la vista columnar a utilizar para mostrar objetos en la lista de resultados.
        /// </summary>
        public ViewBase? ColumnsView
        {
            get
            {
                if (!(CrudElement.GetDescription(ActiveModel).ListColumns is { } c)) return null;
                var v = new GridView();
                foreach (var j in c)
                {
                    v.Columns.Add(j);
                }
                return v;
            }
        }

        /// <summary>
        ///     Obtiene una colección con los resultados de la búsqueda.
        /// </summary>
        public ICollectionView? Results
        {
            get => _results;
            private set => Change(ref _results, value);
        }

        /// <summary>
        ///     Obtiene un valor que indica si al ejecutar
        ///     <see cref="SearchCommand"/> se hará una búsqueda o se limpiará
        ///     la búsqueda actual.
        /// </summary>
        public bool WillSearch
        {
            get => _canSearch;
            private set => Change(ref _canSearch, value);
        }

        /// <summary>
        ///     Obtiene la etiqueta a utilizar para mostrar sobre el botón de
        ///     búsqueda.
        /// </summary>
        public string SearchLabel => WillSearch ? "❌" : "🔍";

        /// <summary>
        ///     Obtiene el comando relacionado a la acción Search.
        /// </summary>
        /// <returns>El comando Search.</returns>
        public ObservingCommand SearchCommand { get; }

        /// <summary>
        ///     Obtiene la etiqueta del modelo para el cual se ha construido 
        ///     este control.
        /// </summary>
        public string ModelLabel { get; }

        /// <summary>
        ///     Obtiene el origen de selección de este <see cref="ListEditorViewModel"/>.
        /// </summary>
        public ICollection<ModelBase> SelectionSource { get; }

        /// <summary>
        ///     Obtiene el comando que agrega elementos desde la lista de
        ///     origen de selección a la colección del modelo de datos.
        /// </summary>
        public SimpleCommand SelectCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand OkSelectCommand { get; }

        /// <summary>
        ///     Obtiene el comando que cancela la adición de elementos
        ///     seleccionados desde una lista.
        /// </summary>
        public ICommand CancelSelectCommand { get; }

        /// <summary>
        ///     Obtiene o establece un valor que configura este
        ///     <see cref="ListEditorViewModel"/> para agregar elementos desde
        ///     una lista.
        /// </summary>
        public bool SelectMode
        {
            get => _selectMode;
            set => Change(ref _selectMode, value);
        }

        public object? TempSelection
        { 
            get=> _tempSelection; 
            set=>Change(ref _tempSelection, value);
        }


        public ObjectEditorViewModel(IObjectPropertyDescription description, params Type[] models) : this(description.Source?.ToList(), description, models) { }

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
            OnCancelSelect();
        }

        public ObjectEditorViewModel(ICollection<ModelBase> selectionSource, IObjectPropertyDescription description, params Type[] models) : base(models)
        {
            FieldName = description.Label;
            FieldIcon = description.Icon;
            CanSelect = description.Selectable;

            SelectionSource = selectionSource;
            SelectCommand = new SimpleCommand(OnSelect);
            OkSelectCommand = new SimpleCommand(OnOkSelect);
            CancelSelectCommand = new SimpleCommand(OnCancelSelect);

            RegisterPropertyChangeBroadcast(nameof(Selection), nameof(DisplayValue));
            RegisterPropertyChangeBroadcast(nameof(ActiveModel), nameof(ColumnsView));
            RegisterPropertyChangeBroadcast(nameof(WillSearch), nameof(SearchLabel));            

            Models = description.ChildModels?.ToList()
                ?? description.PropertyType.Derivates().Select(p => p.ResolveToDefinedType()!).Distinct().Where(TypeExtensions.IsInstantiable).OrNull()?.ToList()
                ?? new[] { description.PropertyType }.ToList();

            ModelLabel = description.Label;
            ActiveModel = Models.FirstOrDefault();
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
        ///     Obtiene un valor que indica si este
        ///     <see cref="ListEditorViewModel"/> permite adicionar elementos
        ///     existentes a la colección del modelo de datos.
        /// </summary>
        public bool CanSelect
        {
            get => _canSelect;
            internal set
            {
                _canSelect = value;
                SelectCommand?.SetCanExecute(value);
            }
        }

        protected override void OnDelete(object o)
        {
            Selection = null;
        }

        protected override Task<DetailedResult> PerformSave(ModelBase entity)
        {
            return Task.FromResult(DetailedResult.Ok);
        }

        protected override ModelBase? GetParent()
        {
            return null;
        }

        protected override void AfterSave()
        {
        }

        /// <summary>
        ///     Obtiene el ícono configurado para mostrar del campo
        ///     correspondiente a la colección subyacente del modelo de datos.
        /// </summary>
        public string FieldIcon
        {
            get => _fieldIcon;
            internal set => Change(ref _fieldIcon, value);
        }

        /// <summary>
        ///     Obtiene un valor amigable para mostrar en la UI de la aplicación.
        /// </summary>
        public string DisplayValue => Selection?.ToString() ?? "-";

        /// <summary>
        ///     Obtiene el nombre configurado para mostrar del campo
        ///     correspondiente a la colección subyacente del modelo de datos.
        /// </summary>
        public string FieldName
        {
            get => _fieldName;
            internal set => Change(ref _fieldName, value);
        }

        /// <summary>
        ///     Limpia los resultados de la búsqueda.
        /// </summary>
        public void ClearSearch()
        {
            Results = null;
            SearchQuery = null;
        }

        /// <summary>
        ///     Obtiene o establece el valor IsSearching.
        /// </summary>
        /// <value>El valor de IsSearching.</value>
        public bool IsSearching
        {
            get => _isSearching;
            set => Change(ref _isSearching, value);
        }

        private async void OnSearch()
        {
            Selection = null;
            if (WillSearch && !SearchQuery.IsEmpty() && ActiveModel != null) await PerformSearch();
            else ClearSearch();
        }

        private async Task PerformSearch()
        {
            if (ActiveModel is null) return;

            var s = SearchQuery!.ToLower();
            var f = new List<IFilter>();

            if (ActiveModel.Implements<ISoftDeletable>())
            {
                f.Add(new EqualsFilter()
                {
                    Property = ActiveModel.GetProperty("IsDeleted")!,
                    Value = false.ToString()
                });
            }
            if (ActiveModel.Implements<INameable>())
            {
                f.Add(new ContainsFilter()
                {
                    Property = ActiveModel.GetProperty("Name")!,
                    Value = s
                });
            }
            if (ActiveModel.Implements<IDescriptible>())
            {
                f.Add(new ContainsFilter()
                {
                    Property = ActiveModel.GetProperty("Description")!,
                    Value = s
                });
            }
            if (ActiveModel.Implements<IUserBase>())
            {
                f.Add(new ContainsFilter()
                {
                    Property = ActiveModel.GetProperty("UserId")!,
                    Value = s
                });
            }
            if (ActiveModel.Implements<ITitledText>())
            {
                f.Add(new EqualsFilter()
                {
                    Property = ActiveModel.GetProperty("Header")!,
                    Value = s
                });
            }

            f.Add(new EqualsFilter()
            {
                Property = ActiveModel.GetProperties().First(p=>p.Name == "Id"),
                Value = s
            });

            var q = QueryBuilder.BuildQuery(ActiveModel, f);
            IsSearching = true;
            var r = await q.ToListAsync();

            Results = CollectionViewSource.GetDefaultView(r);
            Results.Refresh();

            IsSearching = false;
            WillSearch = false;
        }
    }
}