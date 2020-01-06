/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Reporting;
using TheXDS.Proteus.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Documents;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using static TheXDS.MCART.Objects;
using System.Windows;
using System.Windows.Controls;

namespace TheXDS.Proteus.ViewModels
{
    public class ModelInfo
    {
        public Type Model { get; }
        public string Name { get; }

        public ModelInfo(Type model)
        {
            Model = model;
            Name = Crud.CrudElement.GetDescription(model)?.FriendlyName ?? model.NameOf();
        }
    }

    public class GenericReportViewModel : PageViewModel
    {
        private ModelInfo? _model;
        private FlowDocument? _actualReport;

        /// <summary>
        ///     Enumera los modelos disponibles para generar reportes.
        /// </summary>
        public IEnumerable<ModelInfo> Models => GetTypes<ModelBase>(true).Select(p => p.ResolveToDefinedType()).Distinct().NotNull().Select(p => new ModelInfo(p));

        /// <summary>
        ///     Filtros a aplicar al reporte.
        /// </summary>
        public ObservableCollection<ReportFilterViewModel> Filters { get; } = new ObservableCollection<ReportFilterViewModel>();

        /// <summary>
        ///     Columnas del reporte.
        /// </summary>
        public ObservableCollection<ColumnSelectionViewModel> Columns { get; } = new ObservableCollection<ColumnSelectionViewModel>();

        /// <summary>
        ///     Obtiene el comando relacionado a la acción AddFilter.
        /// </summary>
        /// <returns>El comando AddFilter.</returns>
        public SimpleCommand AddFilterCommand { get; }

        private void OnAddFilter()
        {
            Filters.Add(new ReportFilterViewModel(this));
        }

        /// <summary>
        ///     Obtiene o establece el valor Model.
        /// </summary>
        /// <value>El valor de Model.</value>
        public ModelInfo? Model
        {
            get => _model;
            set
            {
                if (!Change(ref _model, value)) return;
                Filters.Clear();
                Columns.Clear();
                if (value is null) return;
                foreach (var j in ValidProperties ?? Array.Empty<PropertyInfo>())
                {
                    Columns.Add(new ColumnSelectionViewModel(j));
                }
            }
        }

        public IEnumerable<PropertyInfo>? ValidProperties => Model?.Model.GetProperties().Where(IsValidProperty).Reverse();
        private bool IsValidProperty(PropertyInfo p)
        {
            return p.CanRead && p.CanWrite 
                && p.DeclaringType != typeof(ModelBase)
                && (!p.PropertyType.Implements<IEnumerable>()|| p.PropertyType == typeof(string));
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="GenericReportViewModel"/>.
        /// </summary>
        /// <param name="host"></param>
        public GenericReportViewModel(ICloseable host) : base(host)
        {
            AddFilterCommand = new SimpleCommand(OnAddFilter);
            GenerateCommand = new SimpleCommand(OnGenerate);
            PrintCommand = new SimpleCommand(OnPrint);
            RegisterPropertyChangeBroadcast(nameof(Model), nameof(ValidProperties));
        }

        /// <summary>
        ///     Obtiene el comando relacionado a la acción Generate.
        /// </summary>
        /// <returns>El comando Generate.</returns>
        public SimpleCommand GenerateCommand { get; }


        /// <summary>
        ///     Obtiene el comando relacionado a la acción Print.
        /// </summary>
        /// <returns>El comando Print.</returns>
        public SimpleCommand PrintCommand { get; }

        private void OnPrint()
        {
            if (ActualReport is null) return;
            var dialog = new PrintDialog();
            if (!dialog.ShowDialog() ?? true) return;
            var sz = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);

            var paginator = (ActualReport as IDocumentPaginatorSource).DocumentPaginator;
            ActualReport.ColumnWidth *= 2;
            paginator.PageSize = sz;
            dialog.PrintDocument(paginator, $"Reporte de {Model!.Name} - Proteus");
        }

        private void OnGenerate()
        {
            var q = QueryBuilder.BuildQuery(Model!.Model, Filters.Select(p=>p.Entity));
            var fd = ReportBuilder.MakeReport($"Reporte de {Model.Name}");
            ReportBuilder.MakeTable(fd, q, Columns.Where(p => p.Selected).Select(p => p.Property));
            ActualReport = fd;
        }

        /// <summary>
        ///     Obtiene o establece el valor ActualReport.
        /// </summary>
        /// <value>El valor de ActualReport.</value>
        public FlowDocument? ActualReport
        {
            get => _actualReport;
            set => Change(ref _actualReport, value);
        }
    }
}