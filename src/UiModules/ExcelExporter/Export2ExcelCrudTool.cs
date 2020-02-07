/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART;
using TheXDS.MCART.Resources;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Plugins
{
	/// <summary>
	/// Herramienta que permite generar reportes en Excel a partir de la tabla de datos activa en la vista de Crud.
	/// </summary>
	public class Export2ExcelCrudTool : CrudTool
	{
		/// <summary>
		/// Inicializa una nueva instancia de la clase
		/// <see cref="Export2ExcelCrudTool"/>.
		/// </summary>
		public Export2ExcelCrudTool() : base(CrudToolVisibility.Unselected)
		{
		}

		/// <summary>
		/// Obtiene una colección de <see cref="Launcher"/> que expone la acción de 
		/// exportar la lista actual a una hoja de cálculo de Microsoft Excel.
		/// </summary>
		/// <param name="models">
		/// Modelos a utilizar para construir los botones de exportación.
		/// </param>
		/// <param name="vm">
		/// Instancia de ViewModel donde se expondrán los <see cref="Launcher"/>
		/// creados.
		/// </param>
		/// <returns>
		/// Una colección de <see cref="Launcher"/> que expone la acción de  exportar
		/// la lista actual a una hoja de cálculo de Microsoft Excel.
		/// </returns>
		public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel vm)
		{
			return models.Select(j => new Launcher(
				"Exportar a Excel",
				"Exporta la lista actual a un archivo de Microsoft Excel.",
				ReflectionHelpers.GetMethod<Export2ExcelCrudTool, Action<ICrudViewModel, Type>>(p => p.Export).FullName(),
				new SimpleCommand(() => vm.BusyDo(Task.Run(() => Export(vm, j)))))
			);
		}

		private void Export(ICrudViewModel vm, Type j)
		{
			var sfd = new SaveFileDialog()
			{ 
				Filter = "Archivo de Excel (*.xlsx)|*.xlsx|Todos los archivos|*.*"
			};
			if (!(CrudElement.GetDescription(j) is { } d)) return;
			if (!(sfd.ShowDialog() ?? false)) return;
			var e = BuildWb(d, out var ws, out var row, out var lastCol);
			foreach (var i in GetQuery(vm, j))
			{
				int col = 1;
				foreach (var l in d.ListColumns)
				{
					ws.Cells[row, col].Value = l.ToString(i);
					col++;
				}
				row++;
			}
			for (int k = 1; k <= lastCol; k++)
			{
				ws.Column(k).AutoFit();
			}
			e.SaveAs(new FileInfo(sfd.FileName));
		}

		private IEnumerable<ModelBase> GetQuery(ICrudViewModel vm, Type j)
		{
			return (vm as ISearchViewModel)?.EnumerableResults
				?? (vm as ICrudCollectionViewModel)?.Source
				?? Proteus.Infer(j)?.All(j).ToList()
				?? throw new InvalidOperationException();
		}

		private ExcelPackage BuildWb(ICrudDescription description, out ExcelWorksheet ws, out int row, out int lastCol)
		{
			var e = new ExcelPackage();
			ws = e.Workbook.Worksheets.Add(description.FriendlyName);
			ws.Cells[1, 1].Value = Proteus.Settings?.BusinessName ?? string.Empty;
			ws.Cells[2, 1].Value = "Reporte de " + description.FriendlyName;
			ws.Cells[2, 1].Style.Font.Size *= 1.3f;
			ws.Cells[3, 1].Value = string.Format("Reporte generado el {0}", DateTime.Now);
			lastCol = 0;
			foreach (var i in description.ListColumns)
			{
				lastCol++;
				ws.Cells[4, lastCol].Value = i.Header;
			}
			bool flag = lastCol > 0;
			if (flag)
			{
				ws.Cells[4, 1, 4, lastCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
				var backgroundColor = ws.Cells[4, 1, 4, lastCol].Style.Fill.BackgroundColor;
				var lightSlateGray = Colors.LightSlateGray;
				backgroundColor.SetColor(lightSlateGray);
			}
			for (int j = 1; j < 4; j++)
			{
				ws.Cells[j, 1, j, lastCol].Merge = true;
			}
			row = 5;
			return e;
		}
	}
}