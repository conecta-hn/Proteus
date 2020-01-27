/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Models.Base
{
    public interface IModelBase<T> where T : IComparable<T>
    {
        T Id { get; set; }
    }
}