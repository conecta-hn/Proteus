/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Crud.Base
{
    public interface IPropertyNumberDescriptor<T> : IPropertyDescriptor where T : IComparable<T>
    {
        IPropertyNumberDescriptor<T> Range(T min, T max);
        IPropertyNumberDescriptor<T> Format(string format);
    }
}