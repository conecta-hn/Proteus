/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows;
using System;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud.Base
{
    public class CustomColum : IColumn
    {
        public string? Format { get; }
        public string Header { get; }
        public GridLength Width { get; }
        public Func<Type, ModelBase, object?> ValueDelegate { get; }

        public CustomColum(string header, GridLength width, string? format, Func<Type, ModelBase, object?> valueDelegate)
        {
            Header = header;
            Width = width;
            Format = format;
            ValueDelegate = valueDelegate;
        }

        public CustomColum(string header, GridLength width, string? format, Func<ModelBase, object?> valueDelegate)
            : this(header, width, format, (_, m) => valueDelegate(m)) { }

        public CustomColum(string header, GridLength width, Func<Type, ModelBase, object?> valueDelegate)
            : this(header, width, null, valueDelegate) { }

        public CustomColum(string header, GridLength width, Func<ModelBase, object?> valueDelegate)
            : this(header, width, null, valueDelegate) { }

        public CustomColum(string header, Func<Type, ModelBase, object?> valueDelegate)
            : this(header, GridLength.Auto, null, valueDelegate) { }

        public CustomColum(string header, Func<ModelBase, object?> valueDelegate)
            : this(header, GridLength.Auto, null, valueDelegate) { }

        public CustomColum(string header, string? format, Func<Type, ModelBase, object?> valueDelegate)
            : this(header, GridLength.Auto, format, valueDelegate) { }

        public CustomColum(string header, string? format, Func<ModelBase, object?> valueDelegate)
            : this(header, GridLength.Auto, format, valueDelegate) { }

        public object? GetValue(Type model, ModelBase entity) => ValueDelegate(model, entity);
    }
}