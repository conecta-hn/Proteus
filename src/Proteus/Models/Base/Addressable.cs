/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Models.Base
{
    public abstract class Addressable : Addressable<Guid>
    {
    }

    public abstract class Addressable<T> : Contact<T>, IAddressable where T : IComparable<T>
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
}
