/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Reflection;

namespace TheXDS.Proteus.Crud.Base
{
    internal class DatePropertyDescriptor : CrudNumericPropertyDescriptor<DateTime>, IPropertyDateDescription, IPropertyDateDescriptor
    {
        public DatePropertyDescriptor(PropertyInfo property) : base(property)
        {
        }

        public bool WithTime { get; private set; }

        IPropertyDateDescriptor IPropertyDateDescriptor.WithTime()
        {
            WithTime = true;
            return this;
        }
    }
}