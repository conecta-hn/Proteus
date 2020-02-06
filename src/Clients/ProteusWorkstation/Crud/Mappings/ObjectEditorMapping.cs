/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.Widgets;
using System;
using System.Linq;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Crud.Mappings
{
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

        public override object? ControlValue
        {
            get
            {
                return _vm.Selection;
            }

            set
            {
                _vm.Selection = value;
            }
        }

        public override void ClearControlValue()
        {
            ControlValue = null;
        }

        private Type[] GetModels(Type baseModel)
        {
            return new Type?[1] { baseModel.ResolveToDefinedType()!.IsInstantiable() ? baseModel : null }
                .Concat(baseModel.Derivates()
                    .Select(p => p.ResolveToDefinedType()!)
                    .Where(p => p.IsInstantiable()))
                .Distinct()
                .NotNull()
                .ToArray();
        }
    }
}