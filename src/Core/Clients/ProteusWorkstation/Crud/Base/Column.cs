/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Mappings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TheXDS.Proteus.Crud.Base
{
    public class Column
    {
        private static GridLength _defaultGrid => new GridLength(1, GridUnitType.Auto);
        public string Header { get; }
        public string Path { get; }
        public string Format { get; internal set; }
        public GridLength Width { get; }

        public Column(string path) : this(path, path, _defaultGrid) { }
        public Column(string path, GridLength width):this(path,path,width) { }
        public Column(string header, string path) : this(header, path, _defaultGrid) { }
        public Column(string header, string path, GridLength width)
        {
            Header = header;
            Path = path;
            Width = width;
        }




        public static implicit operator Column(string path) => new Column(path, path);
        public static implicit operator string(Column column) => column.Header;
        public static implicit operator GridLength(Column column) => column.Width;
        public static implicit operator double(Column column)=> column.Width.IsAbsolute ? column.Width.Value : double.NaN;
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