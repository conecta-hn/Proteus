/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Representa una columna de tabla que puede ser convertira a diversos
    /// objetos.
    /// </summary>
    public class Column : IColumn
    {
        private static GridLength DefaultGrid => new GridLength(1, GridUnitType.Auto);

        /// <summary>
        /// Obtiene el encabezado a mostrar en la tabla.
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// Obtiene la ruta de propiedad a obtener para el valor de esta
        /// columna.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Obtiene un valor que indica el formato a utilizar para representar
        /// el valor de esta columna.
        /// </summary>
        public string Format { get; internal set; }

        /// <summary>
        /// Obtiene un valor que indica el ancho sugerido de esta columna.
        /// </summary>
        public GridLength Width { get; }

        public Column(string path) : this(path, path, DefaultGrid) { }
        public Column(string path, GridLength width) : this(path, path, width) { }
        public Column(string header, string path) : this(header, path, DefaultGrid) { }
        public Column(string header, string path, GridLength width)
        {
            Header = header;
            Path = path;
            Width = width;
        }

        public object? GetValue(Type model, ModelBase entity)
        {
            var propertyInfo = model.GetProperties().FirstOrDefault((PropertyInfo p) => p.Name == Path);
            return propertyInfo?.GetValue(entity);
        }

        public object? GetValue(ModelBase entity)
        {
            return GetValue(entity.GetType(), entity);
        }

        public string? ToString(ModelBase entity)
        {
            return GetStringValue(GetValue(entity), Format);
        }

        private string? GetStringValue(object? obj, string format)
        {
            return format.IsEmpty()
                ? obj?.ToString()
                : obj?.GetType().GetMethod(nameof(ToString), new Type[] { typeof(string) }) is { } i
                    ? i.Invoke(obj, new[] { format }) as string
                    : string.Format(format, obj);
        }


        public static implicit operator Column(string path) => new Column(path, path);
        public static implicit operator string(Column column) => column.Header;
        public static implicit operator GridLength(Column column) => column.Width;
        public static implicit operator double(Column column) => column.Width.IsAbsolute ? column.Width.Value : double.NaN;
        public static implicit operator GridViewColumn(Column column)
        {
            var b = new Binding(column.Path)
            {
                StringFormat = column.Format,
                Mode = BindingMode.OneWay,
                Converter = new ReadOnlyPresenter()
            };

            return new GridViewColumn()
            {
                Header = column.Header,
                DisplayMemberBinding = b,
                HeaderStringFormat = column.Format,
                Width = column.Width.IsAbsolute ? column.Width.Value : double.NaN
            };
        }
    }
}