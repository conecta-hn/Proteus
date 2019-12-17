/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Reflection;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.ViewModels
{
    public class ColumnSelectionViewModel : ViewModelBase
    {
        private bool _selected = true;

        /// <summary>
        ///     Obtiene o establece el valor Model.
        /// </summary>
        /// <value>El valor de Model.</value>
        public PropertyInfo Property { get; }

        public string PropName => Property.NameOf();

        /// <summary>
        ///     Obtiene o establece el valor Selected.
        /// </summary>
        /// <value>El valor de Selected.</value>
        public bool Selected
        {
            get => _selected;
            set => Change(ref _selected, value);
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ColumnSelectionViewModel"/>.
        /// </summary>
        /// <param name="property">
        ///     Modelo parael cual definir un filtro.
        /// </param>
        public ColumnSelectionViewModel(PropertyInfo property)
        {
            Property = property;
        }
    }
}