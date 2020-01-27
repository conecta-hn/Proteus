/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Crud.Base
{
    internal class CrudPropertyDescriptor : IPropertyDescriptor, IPropertyDescription, IEquatable<CrudPropertyDescriptor>
    {
        private string _label;
        private string _icon;
        private bool _hidden;
        private object? _default;
        private NullMode _nullability;
        private int? _order;
        private bool _readOnly;
        private bool _showInDetails;
        private string _tooltip;
        private readonly Dictionary<DependencyProperty, BindingBase> _customBindings = new Dictionary<DependencyProperty, BindingBase>();
        private readonly List<Func<ModelBase, PropertyInfo, IEnumerable<ValidationError>>> _validators = new List<Func<ModelBase, PropertyInfo, IEnumerable<ValidationError>>>();

        public CrudPropertyDescriptor(PropertyInfo property, PropertyLocation location)
        {
            PropertySource = location;
            Property = property;
            _label = property.NameOf();
            _default = property.GetAttr<DefaultValueAttribute>()?.Value ?? property.GetType().Default();
        }
        public CrudPropertyDescriptor(PropertyInfo property) : this(property, PropertyLocation.Model) { }

        #region IPropertyDescription

        public bool UseDefault { get; private set; }
        public IDictionary<DependencyProperty, BindingBase> CustomBindings => _customBindings;
        public bool IsListColumn { get; private set; }
        public PropertyInfo Property { get; }
        public PropertyLocation PropertySource { get; internal set; }
        public string ReadOnlyFormat { get; private set; }
        public string RadioGroup { get; private set; }
        public bool ShowWatermark { get; private set; }
        Func<ModelBase, PropertyInfo, IEnumerable<ValidationError>> IPropertyDescription.Validator => (m, c) => _validators.SelectMany(p => p.Invoke(m, c));
        string IPropertyDescription.Tooltip => _tooltip;
        int? IPropertyDescription.Order => _order;
        string IPropertyDescription.Icon => _icon;
        string IPropertyDescription.Label => _label.OrNull() ?? Property.Name;
        NullMode IPropertyDescription.Nullability => _nullability;
        object IPropertyDescription.Default => _default;
        bool IPropertyDescription.Hidden => _hidden;
        bool IPropertyDescription.ReadOnly => _readOnly;
        bool IPropertyDescription.ShowInDetails => _showInDetails;
        public Type PropertyType => Property.PropertyType;

        #endregion

        #region IPropertyDescriptor

        public void Hidden()
        {
            _hidden = true;
        }
        public IPropertyDescriptor Label(string label)
        {
            _label = label;
            return this;
        }
        public IPropertyDescriptor Icon(string icon)
        {
            _icon = icon;
            return this;
        }
        public IPropertyDescriptor Icon(char icon)
        {
            _icon = new string(new[] { icon });
            return this;
        }
        public IPropertyDescriptor Default(object value)
        {
            UseDefault = true;
            _default = value;
            return this;
        }
        public IPropertyDescriptor Nullability(NullMode nullability)
        {
            _nullability = nullability;
            return this;
        }
        public IPropertyDescriptor Required()
        {
            return Nullability(NullMode.Required).Validator(CheckNotNull);
        }
        private IEnumerable<ValidationError> CheckNotNull(ModelBase arg1, PropertyInfo arg2)
        {
            if (arg1 is null) yield return new NullValidationError(arg2);
        }
        public IPropertyDescriptor Nullable() => Nullability(NullMode.Nullable);
        public IPropertyDescriptor RadioSelectable() => Nullability(NullMode.Radio);
        public IPropertyDescriptor RadioSelectable(string groupId)
        {
            RadioGroup = groupId;
            return RadioSelectable();
        }
        public bool Equals(CrudPropertyDescriptor other) => Property == other.Property;
        public IPropertyDescriptor Order(int order)
        {
            _order = order;
            return this;
        }
        public IPropertyDescriptor Validator(Func<ModelBase, PropertyInfo, IEnumerable<ValidationError>> validator)
        {
            _validators.Add(validator);
            return this;
        }
        public IPropertyDescriptor Validator<T>(Func<T, PropertyInfo, IEnumerable<ValidationError>> validator) where T : ModelBase
        {
            IEnumerable<ValidationError> CastTest(ModelBase m, PropertyInfo p)
            {
                if (!(m is T e)) return new ValidationError[] { $"Error general de validación para {typeof(T).NameOf()}. No se puede validar un {m?.GetType().NameOf() ?? "objeto de referencia nula (null)"}" };
                return validator(e, p);
            }
            _validators.Add(CastTest);
            return this;
        }
        public void ReadOnly()
        {
            _readOnly = true;
        }
        public void ReadOnly(string format)
        {
            ReadOnly();
            ReadOnlyFormat = format;
        }
        public IPropertyDescriptor Format(string format)
        {
            ReadOnlyFormat = format;
            return this;
        }
        public IPropertyDescriptor Tooltip(string tooltip)
        {
            _tooltip = tooltip;
            return this;
        }
        public IPropertyDescriptor WatermarkAlwaysVisible()
        {
            ShowWatermark = true;
            return this;
        }
        public IPropertyDescriptor Bind(DependencyProperty path, BindingBase binding)
        {
            CustomBindings.Add(path, binding);
            return this;
        }
        public IPropertyDescriptor Bind(DependencyProperty path, string binding)
        {
            return Bind(path, new Binding(binding));
        }
        public IPropertyDescriptor Bind(DependencyProperty path, string binding, object source)
        {
            return Bind(path, new Binding(binding) { Source = source });
        }
        public IPropertyDescriptor Bind(DependencyProperty path, PropertyPath binding)
        {
            return Bind(path, new Binding() { Path = binding });
        }
        public IPropertyDescriptor Bind(DependencyProperty path, PropertyPath binding, object source)
        {
            return Bind(path, new Binding() { Path = binding, Source = source });
        }
        public IPropertyDescriptor AsListColumn()
        {
            IsListColumn = true;
            return this;
        }
        public IPropertyDescriptor ShowInDetails()
        {
            _showInDetails = true;
            return this;
        }

        #endregion
    }
}