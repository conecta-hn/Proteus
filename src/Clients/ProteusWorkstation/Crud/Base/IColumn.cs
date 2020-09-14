/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Globalization;
using System.Windows;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud.Base
{
    public interface IColumn
    {
        string? Format { get; }
        string Header { get; }
        GridLength Width { get; }

        object? GetValue(ModelBase entity)
        {
            return GetValue(entity.GetType(), entity);
        }
        object? GetValue(Type model, ModelBase entity);
        string? ToString(ModelBase entity)
        {
            return GetValue(entity) switch
            {
                IFormattable f when !Format.IsEmpty() => f.ToString(Format, CultureInfo.CurrentCulture),
                null => null,
                object o => o.ToString()
            };
        }
    }
}