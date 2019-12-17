/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

#nullable enable

using System.Windows.Controls;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Windows.Data;
using TheXDS.Proteus.Widgets;
using TheXDS.Proteus.ViewModels;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class SimpleObjectSelectorMapping : PropertyMapping
    {
        private readonly ComboBox _cmb;

        public SimpleObjectSelectorMapping(IPropertyDescription property) : base(property, new ComboBox())
        {
            _cmb = (ComboBox)Control;

            if (property is IObjectPropertyDescription p)
            {
                _cmb.ItemsSource = p.Source.ToList();                
            }
            _cmb.SetBinding(Selector.SelectedItemProperty, new Binding(property.Property.Name));
        }

        public override object ControlValue
        {
            get => _cmb.SelectedItem;
            set { }
        }

        public override void ClearControlValue()
        {
        }
    }

    public class SearchComboMapping : PropertyMapping
    {
        private readonly SearchComboViewModel? _vm;

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="SearchComboMapping"/>.
        /// </summary>
        /// <param name="property">
        ///     Descriptor de propiedad a utilizar para generar el control
        ///     asociado.
        /// </param>
        public SearchComboMapping(IPropertyDescription property) : base(property, new SearchCombo())
        {
            if (property is IObjectPropertyDescription p)
            Control.DataContext = _vm = new SearchComboViewModel(p);

        }

        /// <summary>
        ///     Obtiene o establece el valor asociado a este control.
        /// </summary>
        public override object? ControlValue
        {
            get => _vm?.Selection;
            set
            {
                if (_vm is null) return;
                _vm.ClearSearch();
                _vm.Selection = value;
            }
        }

        /// <summary>
        ///     Limpia el estado de este control.
        /// </summary>
        public override void ClearControlValue()
        {
            ControlValue = null;
        }
    }
}