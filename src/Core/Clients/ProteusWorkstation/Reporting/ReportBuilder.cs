/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Mappings;
using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using TheXDS.MCART.Types.Extensions;
using static TheXDS.MCART.Types.Extensions.FlowDocumentExtensions;
using TheXDS.Proteus.Config;

namespace TheXDS.Proteus.Reporting
{
    /// <summary>
    /// Contiene helpers para la creación de reportes.
    /// </summary>
    public static class ReportBuilder
    {
        private static readonly ReadOnlyPresenter _rop = new ReadOnlyPresenter(false);

        /// <summary>
        /// Crea una tabla a partir de la información brindada.
        /// </summary>
        /// <param name="fd">
        /// <see cref="FlowDocument"/> sobre el cual crear la nueva tabla.
        /// </param>
        /// <param name="query">
        /// Query con la información de la tabla a generar.
        /// </param>
        /// <param name="columns">
        /// Columnas a mostrar.
        /// </param>
        /// <returns>
        /// Un <see cref="Table"/> generado a partir de la información
        /// brindada.
        /// </returns>
        public static Table MakeTable(FlowDocument fd, IQueryable<ModelBase> query, IEnumerable<PropertyInfo> columns)
        {
            var c = columns.ToList();
            var tbl = fd.AddTable(c.Select(p => new KeyValuePair<string, GridLength>(p.NameOf() ?? p.Name, GridLength.Auto)));
            
            foreach (var j in query.ToList())
            {
                tbl.AddRow(c.Select(p => p.GetValue(j)?.ToString()));
            }
            return tbl;
        }

        /// <summary>
        /// Crea una tabla a partir de la información brindada.
        /// </summary>
        /// <param name="fd">
        /// <see cref="FlowDocument"/> sobre el cual crear la nueva tabla.
        /// </param>
        /// <param name="query">
        /// Query con la información de la tabla a generar.
        /// </param>
        /// <param name="columns">
        /// Columnas a mostrar.
        /// </param>
        /// <returns>
        /// Un <see cref="Table"/> generado a partir de la información
        /// brindada.
        /// </returns>
        public static Table MakeTable(FlowDocument fd, IQueryable query, IEnumerable<PropertyInfo> columns)
        {
            var c = columns.ToList();
            var tbl = fd.AddTable(c.Select(p => new KeyValuePair<string, GridLength>(p.NameOf() ?? p.Name, GridLength.Auto)));

            foreach (var j in query)
            {
                tbl.AddRow(c.Select(p => _rop.ToString(p.GetValue(j))));
            }

            return tbl;
        }

        /// <summary>
        /// Genera un <see cref="FlowDocument"/> con encabezado estándar
        /// para utilizar al crear un reporte.
        /// </summary>
        /// <param name="title">Título del reporte</param>
        /// <returns>
        /// Un <see cref="FlowDocument"/> con encabezado estándar.
        /// </returns>
        public static FlowDocument MakeReport(string title)
        {
            return new FlowDocument
            {
                FontFamily = new FontFamily("Consolas"),
                FontSize = 11,
                ColumnWidth = 700,
                IsColumnWidthFlexible = true
            }
                .Title(Settings.Default.BusinessName, 3)
                .Title(title)
                .Text($"Reporte generado el {DateTime.Now}");
        }
    }
}
