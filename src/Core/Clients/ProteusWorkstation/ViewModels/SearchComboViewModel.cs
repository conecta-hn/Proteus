/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Reporting;
using static TheXDS.MCART.Types.Extensions.ObservingCommandExtensions;
using static TheXDS.MCART.Types.Extensions.StringExtensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.ViewModels
{
    /// <summary>
    ///     Define la lógica de interacción para un control que permita realizar búsquedas.
    /// </summary>
    public class SearchComboViewModel : ViewModelBase
    {
        private Type? _activeModel;
        private string? _searchQuery;
        private bool _canSearch;
        private object? _selection;
        private bool _isOpen;

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
        public ICollectionView? Results { get; private set; }

        /// <summary>
        ///     Obtiene o establece el valor Selection.
        /// </summary>
        /// <value>El valor de Selection.</value>
        public object? Selection
        {
            get => _selection;
            set => Change(ref _selection, value);
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
        public string SearchLabel => WillSearch ? "🔍" : "❌";

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
        ///     Obtiene o establece el valor IsOpen.
        /// </summary>
        /// <value>El valor de IsOpen.</value>
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (!Change(ref _isOpen, value))return;
                var tmp = Selection;
                if (!value) ClearSearch();
                Selection = tmp;
            }
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="SearchComboViewModel"/>.
        /// </summary>
        /// <param name="description">
        ///     Descriptor de modelo a utilizar para inicializar los controles
        ///     de búsqueda.
        /// </param>
        public SearchComboViewModel(IObjectPropertyDescription description)
        {
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

        private async void OnSearch()
        {
            Selection = null;
            if (WillSearch && !SearchQuery.IsEmpty() && ActiveModel != null) await PerformSearch();
            else ClearSearch();
        }

        /// <summary>
        ///     Limpia los resultados de la búsqueda.
        /// </summary>
        public void ClearSearch()
        {
            Results = null;
            SearchQuery = null;
        }

        private async Task PerformSearch()
        {
            if (ActiveModel is null) return;

            var s = SearchQuery!.ToUpper();
            var f = new List<IFilter>();
            if (ActiveModel.Implements<INameable>())
            {
                f.Add(new ContainsFilter()
                {
                    Property = ActiveModel.GetProperty("Name"),
                    Value = s
                });
            }





            var q = QueryBuilder.BuildQuery(ActiveModel, f);
            IsBusy = true;
            Results = CollectionViewSource.GetDefaultView(await q.ToListAsync());
            Results.Refresh();
            IsBusy = false;
            WillSearch = false;
        }
    }
}