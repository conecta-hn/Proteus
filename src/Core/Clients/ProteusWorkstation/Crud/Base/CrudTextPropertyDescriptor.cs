/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Reflection;

namespace TheXDS.Proteus.Crud.Base
{
    internal class CrudTextPropertyDescriptor : CrudPropertyDescriptor, IPropertyTextDescription, IPropertyTextDescriptor
    {
        public CrudTextPropertyDescriptor(PropertyInfo property) : base(property)
        {
        }

        public int MinLength { get; private set; }
        public int MaxLength { get; private set; }
        public TextKind Kind { get; private set; }
        public string Mask { get; private set; }

        IPropertyTextDescriptor IPropertyTextDescriptor.Big()
        {
            Kind = TextKind.Big;
            return this;
        }

        IPropertyTextDescriptor IPropertyTextDescriptor.Mask(string mask)
        {
            Mask = mask;
            Kind = TextKind.Maskable;
            return this;
        }

        IPropertyTextDescriptor IPropertyTextDescriptor.MaxLength(int maxLength)
        {
            MaxLength = maxLength;
            return this;
        }

        IPropertyTextDescriptor IPropertyTextDescriptor.MinLength(int minLength)
        {
            MinLength = minLength;
            return this;
        }

        IPropertyTextDescriptor IPropertyTextDescriptor.TextKind(TextKind kind)
        {
            Kind = kind;            
            return this;
        }
    }
}