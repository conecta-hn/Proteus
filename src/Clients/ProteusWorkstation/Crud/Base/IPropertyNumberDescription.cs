/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Crud.Base
{
    public interface IPropertyNumberDescription<T> : IPropertyDescription where T : IComparable<T>
    {
        TheXDS.MCART.Types.Range<T>? Range { get; }
        string Format { get; }
    }
}