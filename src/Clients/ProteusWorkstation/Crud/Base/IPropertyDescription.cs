/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace TheXDS.Proteus.Crud.Base
{
    public interface IPropertyDescription
    {
        PropertyLocation PropertySource { get; }
        PropertyInfo Property { get; }
        Type PropertyType { get; }
        object Default { get; }
        bool Hidden { get; }
        bool ReadOnly { get; }
        bool ShowInDetails { get; }
        bool ShowWatermark { get; }
        string ReadOnlyFormat { get; }
        string Icon { get; }
        string Label { get; }
        NullMode Nullability { get; }
        string RadioGroup { get; }
        int? Order { get; }
        Func<ModelBase, PropertyInfo, IEnumerable<ValidationError>> Validator { get; }
        string Tooltip { get; }
        IDictionary<DependencyProperty, BindingBase> CustomBindings { get; }
        bool IsListColumn { get; }
        bool UseDefault { get; }
    }
}