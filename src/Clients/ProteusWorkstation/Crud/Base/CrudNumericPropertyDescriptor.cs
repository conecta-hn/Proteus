/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Reflection;
using TheXDS.MCART.Types;

namespace TheXDS.Proteus.Crud.Base
{
    internal class CrudNumericPropertyDescriptor<T> : CrudPropertyDescriptor, IPropertyNumberDescription<T>, IPropertyNumberDescriptor<T> where T : IComparable<T>
    {
        public CrudNumericPropertyDescriptor(PropertyInfo property) : base(property)
        {
        }

        public Range<T>? Range { get; private set; }

        public string Format { get; private set; }

        IPropertyNumberDescriptor<T> IPropertyNumberDescriptor<T>.Format(string format)
        {
            Format = format;
            return this;
        }

        IPropertyNumberDescriptor<T> IPropertyNumberDescriptor<T>.Range(T min, T max)
        {
            Range = new Range<T>(min, max);
            return this;
        }
    }
}