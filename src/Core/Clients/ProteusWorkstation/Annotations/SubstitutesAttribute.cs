/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Annotations
{
    public sealed class SubstitutesAttribute : Attribute, IValueAttribute<Type>
    {
        public Type Value { get; }
        public SubstitutesAttribute(Type substitutedType)
        {
            Value = substitutedType;
        }
    }
}