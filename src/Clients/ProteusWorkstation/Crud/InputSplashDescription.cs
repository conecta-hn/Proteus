/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using TheXDS.Proteus.Models.Base;
using System.Windows.Data;

namespace TheXDS.Proteus.Crud
{
    internal struct InputSplashDescription : IPropertyDescription
    {
        public PropertyLocation PropertySource => PropertyLocation.ViewModel;
        public bool Hidden => false;
        public bool ReadOnly => false;
        public bool ShowInDetails => false;
        public bool ShowWatermark => true;
        public string ReadOnlyFormat => null;
        public NullMode Nullability => NullMode.Required;
        public Type PropertyType => Property.PropertyType;
        public string RadioGroup => null;
        public int? Order => null;
        public Func<ModelBase, PropertyInfo, IEnumerable<ValidationError>> Validator => null;
        public IDictionary<DependencyProperty, BindingBase> CustomBindings => null;
        public bool IsListColumn => false;
        public bool UseDefault => !(Default is null);
        public object Default { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
        public PropertyInfo Property { get; set; }
        public string Tooltip { get; set; }
    }
}