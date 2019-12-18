/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using System.Windows;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class ListMapping : PropertyMapping
    {
        private readonly ListEditorViewModel _vm;
        public ListMapping(IPropertyDescription property) : base(property, new ListEditor())
        {
            if (property is IListPropertyDescription i)
            {
                var t = i.Property.PropertyType.ResolveCollectionType().ResolveToDefinedType();
                _vm = t.IsAbstract && i.Creatable
                    ? new ListEditorViewModel(i, GetModels(t))
                    : new ListEditorViewModel(i);
                _vm.Unselected += ((ListEditor)Control).ClearSelection;
                Control.DataContext = _vm;
            }
        }

        private Type[] GetModels(Type baseModel)
        {
            return baseModel.Derivates()
                .Select(p => p.ResolveToDefinedType()!)
                .Where(p=> p.IsInstantiable())
                .Distinct()
                .ToArray();
        }

        public override object ControlValue
        {
            get => _vm.Source;
            set => _vm.Source = new ObservableCollectionWrap<ModelBase>(((IEnumerable)value)?.ToGeneric().OfType<ModelBase>().ToList() ?? new List<ModelBase>());
        }

        public override void ClearControlValue()
        {
            if (!_vm.Source.IsReadOnly) _vm.Source.Clear();
        }
    }

    public class ObjectEditorMapping : PropertyMapping
    {
        private readonly ObjectEditorViewModel _vm;

        public ObjectEditorMapping(IPropertyDescription property) : base(property, new ObjectEditor())
        {
            if (property is IObjectPropertyDescription i)
            {
                var t = i.Property.PropertyType.ResolveToDefinedType()!;
                _vm = new ObjectEditorViewModel(i, GetModels(t));
                Control.DataContext = _vm;
            }

        }

        public override object ControlValue
        { 
            get => throw new NotImplementedException();
            set => throw new NotImplementedException(); 
        }

        public override void ClearControlValue()
        {
            throw new NotImplementedException();
        }

        private Type[] GetModels(Type baseModel)
        {
            return baseModel.Derivates()
                .Select(p => p.ResolveToDefinedType()!)
                .Where(p => p.IsInstantiable())
                .Distinct()
                .ToArray();
        }

    }
}