/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Reporting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheXDS.MCART.ViewModel;
using static TheXDS.MCART.Objects;

namespace TheXDS.Proteus.ViewModels
{
    public class ReportFilterViewModel : ViewModel<IPropComparisonFilter>
    {
        private PropertyInfo _field;
        private string _value;

        public static IEnumerable<IPropComparisonFilter> Filters { get; } = FindAllObjects<IPropComparisonFilter>().ToList();

        /// <summary>
        ///     Enumera las propiesdades del modelo de datos.
        /// </summary>
        public IEnumerable<PropertyInfo> Properties => Parent.ValidProperties;

        /// <summary>
        ///     Obtiene o establece el valor Model.
        /// </summary>
        /// <value>El valor de Model.</value>
        public GenericReportViewModel Parent { get; }

        /// <summary>
        ///     Obtiene o establece el valor Field.
        /// </summary>
        /// <value>El valor de Field.</value>
        public PropertyInfo Field
        {
            get => Entity?.Property ?? _field;
            set
            {
                if (!Change(ref _field, value)) return;
                if (Entity is null) return;
                Entity.Property = value;
            }
        }

        public IPropComparisonFilter Comparison
        {
            get => Entity;
            set
            {
                Entity = value;
                Notify();
                if (Entity is null) return;
                Entity.Property = _field;
                Entity.Value = _value;
            }
        }

        /// <summary>
        ///     Obtiene o establece el valor Value.
        /// </summary>
        /// <value>El valor de Value.</value>
        public string Value
        {
            get => Entity?.Value ?? _value;
            set
            {
                if (!Change(ref _value, value)) return;
                if (Entity is null) return;
                Entity.Value = value;
            }
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ReportFilterViewModel"/>.
        /// </summary>
        /// <param name="parent">
        ///     Modelo parael cual definir un filtro.
        /// </param>
        public ReportFilterViewModel(GenericReportViewModel parent)
        {
            Parent = parent;
            RemoveCommand = new SimpleCommand(OnRemove);
        }

        /// <summary>
        ///     Obtiene el comando relacionado a la acción Remove.
        /// </summary>
        /// <returns>El comando Remove.</returns>
        public SimpleCommand RemoveCommand { get; }

        private void OnRemove()
        {
            Parent.Filters.Remove(this);
        }
    }
}