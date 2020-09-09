/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.Widgets;

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
                _vm = t.IsAbstract
                    ? new ListEditorViewModel(i, AppInternal.GetModels(t))
                    : new ListEditorViewModel(i);
                _vm.Unselected += ((ListEditor)Control).ClearSelection;
                Control.DataContext = _vm;
            }
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
}