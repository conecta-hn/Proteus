/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Models.Base
{
    public abstract class Valuable<T> : ModelBase<T>, IValuable where T : IComparable<T>
    {
        public decimal? StaticValue { get; set; }
        public float? PercentValue { get; set; }
    }
}